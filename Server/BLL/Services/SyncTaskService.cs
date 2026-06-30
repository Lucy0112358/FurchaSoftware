using FurchaBLL.Constants;
using FurchaBLL.Models;
using FurchaDAL.Models;
using FurchaDAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurchaBLL.Services
{
    /// <summary>
    /// Queue mechanics for the cloud→brain MQTT outbox (furcha.SyncTask).
    /// Built on the generic <see cref="BaseRepository{T}"/> for CRUD and EF Core's
    /// <c>ExecuteUpdateAsync</c> for set-based status transitions (single SQL UPDATE, no entities
    /// loaded into memory). No raw SQL — the claim uses an optimistic compare-and-swap.
    /// All timestamps are <see cref="DateTime.UtcNow"/> (UTC).
    ///
    /// Payload building / routing (which brains a user reaches) is a separate concern (Task #5);
    /// this service stores and moves tasks, it does not build their payloads.
    /// </summary>
    public class SyncTaskService
    {
        private readonly BaseRepository<SyncTask> _repo;
        private furchaContext Db => _repo.Context;

        // A task left in "Sent" longer than this is considered stuck and released for retry.
        private const int DefaultSentTimeoutSeconds = 30;

        // Cap exponential backoff so a perpetually-failing task still retries roughly hourly.
        private const int MaxBackoffSeconds = 3600;

        // How many times the optimistic claim retries when it loses a compare-and-swap race.
        private const int MaxClaimRetries = 3;

        public SyncTaskService(BaseRepository<SyncTask> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Insert a new task. For deltas (a row tied to a specific entity via <see cref="SyncTask.EntityId"/>),
        /// any older still-Pending task for the same (BrainUid, EntityType, EntityId) is first marked
        /// Superseded so the brain only receives the latest state.
        ///
        /// Composes with an ambient transaction: if the caller has already opened one on the same
        /// furchaContext (e.g. the API writing user data + this task atomically — Task #6), this method
        /// enlists in it. Otherwise it wraps the coalesce+insert in its own transaction.
        /// </summary>
        public async Task<SyncTask> EnqueueAsync(SyncTask task)
        {
            var ownTransaction = Db.Database.CurrentTransaction is null
                ? await Db.Database.BeginTransactionAsync()
                : null;

            try
            {
                // Coalesce only deltas (snapshots have no EntityId and must not be collapsed).
                if (task.EntityId.HasValue)
                {
                    await Db.SyncTasks
                        .Where(t => t.Status == (byte)SyncTaskStatus.Pending
                                 && t.BrainUid == task.BrainUid
                                 && t.EntityType == task.EntityType
                                 && t.EntityId == task.EntityId)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(t => t.Status, (byte)SyncTaskStatus.Superseded));
                }

                await _repo.InsertAsync(task);

                if (ownTransaction is not null)
                    await ownTransaction.CommitAsync();

                return task;
            }
            catch
            {
                if (ownTransaction is not null)
                    await ownTransaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (ownTransaction is not null)
                    await ownTransaction.DisposeAsync();
            }
        }

        /// <summary>
        /// Claim the next deliverable task for a single brain and mark it Sent, using an optimistic
        /// compare-and-swap (no raw SQL, no table hints). Enforces:
        ///   - per-brain FIFO (oldest claimable Id first)
        ///   - "one in-flight message per brain" (won't claim while a Sent row exists for the brain)
        ///   - Attempts &lt; MaxAttempts (never claims an exhausted row; keeps Attempts ≤ MaxAttempts)
        ///
        /// The claim is a single atomic UPDATE whose WHERE re-checks the status and the in-flight gate,
        /// so two workers can never both flip a row to Sent for the same brain — the loser gets
        /// affected == 0 and retries. Returns null when there is nothing to send for this brain.
        /// </summary>
        public async Task<ClaimedSyncTask?> ClaimNextAsync(Guid accountUid, string brainUid)
        {
            for (var retry = 0; retry < MaxClaimRetries; retry++)
            {
                var now = DateTime.UtcNow;

                // 1) Pick the oldest claimable candidate for this brain, only if nothing is in flight.
                var candidateId = await Db.SyncTasks
                    .Where(t => t.AccountUid == accountUid
                             && t.BrainUid == brainUid
                             && (t.Status == (byte)SyncTaskStatus.Pending
                              || t.Status == (byte)SyncTaskStatus.Failed)
                             && t.Attempts < t.MaxAttempts
                             && t.NextAttemptAt <= now
                             && !Db.SyncTasks.Any(s => s.Status == (byte)SyncTaskStatus.Sent
                                                    && s.AccountUid == accountUid
                                                    && s.BrainUid == brainUid))
                    .OrderBy(t => t.Id)
                    .Select(t => (long?)t.Id)
                    .FirstOrDefaultAsync();

                if (candidateId is null)
                    return null; // nothing waiting for this brain

                // 2) Atomic compare-and-swap: claim THAT row only if it is still claimable and the
                //    in-flight gate is still clear. The row lock taken by this UPDATE serializes
                //    competing workers — exactly one gets affected == 1.
                var affected = await Db.SyncTasks
                    .Where(t => t.Id == candidateId.Value
                             && (t.Status == (byte)SyncTaskStatus.Pending
                              || t.Status == (byte)SyncTaskStatus.Failed)
                             && t.Attempts < t.MaxAttempts
                             && !Db.SyncTasks.Any(s => s.Status == (byte)SyncTaskStatus.Sent
                                                    && s.AccountUid == accountUid
                                                    && s.BrainUid == brainUid))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(t => t.Status, (byte)SyncTaskStatus.Sent)
                        .SetProperty(t => t.Attempts, t => t.Attempts + 1)
                        .SetProperty(t => t.SentAt, now));

                if (affected == 0)
                    continue; // lost the race or the gate closed — try again

                // 3) Read back the claimed row for the worker to publish.
                return await Db.SyncTasks
                    .Where(t => t.Id == candidateId.Value)
                    .Select(t => new ClaimedSyncTask
                    {
                        Id = t.Id,
                        AccountUid = t.AccountUid,
                        BrainUid = t.BrainUid,
                        CommandType = t.CommandType,
                        Operation = t.Operation,
                        Payload = t.Payload
                    })
                    .FirstOrDefaultAsync();
            }

            return null; // gave up after losing repeated claim races (heavy contention)
        }

        /// <summary>
        /// Close a task as confirmed by the brain. Idempotent: only transitions a row still in Sent,
        /// so a duplicated ACK (QoS 1 redelivery) is a no-op. Returns true if this call closed the row.
        /// </summary>
        public async Task<bool> MarkAckedAsync(long taskId)
        {
            var affected = await Db.SyncTasks
                .Where(t => t.Id == taskId && t.Status == (byte)SyncTaskStatus.Sent)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Status, (byte)SyncTaskStatus.Acked)
                    .SetProperty(t => t.AckedAt, DateTime.UtcNow));

            return affected > 0;
        }

        /// <summary>
        /// Mark a send/ACK failure on a single task. Applies exponential backoff (2^Attempts seconds,
        /// capped) and transitions to Dead once attempts are exhausted. Attempts is already incremented
        /// at claim time, so the backoff is computed from the post-claim value.
        /// </summary>
        public async Task MarkFailedAsync(long taskId, string? error)
        {
            var task = await _repo.GetAsync(taskId);
            if (task is null)
                return;

            task.Status = (byte)(task.Attempts >= task.MaxAttempts
                ? SyncTaskStatus.Dead
                : SyncTaskStatus.Failed);
            task.LastError = Truncate(error, 400);

            var backoffSeconds = (int)Math.Min(Math.Pow(2, task.Attempts), MaxBackoffSeconds);
            task.NextAttemptAt = DateTime.UtcNow.AddSeconds(backoffSeconds);

            await _repo.UpdateAsync(task);
        }

        /// <summary>
        /// Release tasks stuck in Sent (no ACK within the timeout) back to Failed, or Dead if exhausted.
        /// Frees the per-brain in-flight gate so the next task can go. Returns the number released.
        /// Intended to be called periodically by the worker.
        /// </summary>
        public async Task<int> TimeoutStuckSentAsync(int timeoutSeconds = DefaultSentTimeoutSeconds)
        {
            var now = DateTime.UtcNow;

            return await Db.SyncTasks
                .Where(t => t.Status == (byte)SyncTaskStatus.Sent
                         && t.SentAt != null
                         && EF.Functions.DateDiffSecond(t.SentAt.Value, now) > timeoutSeconds)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Status, t => t.Attempts >= t.MaxAttempts
                        ? (byte)SyncTaskStatus.Dead
                        : (byte)SyncTaskStatus.Failed)
                    .SetProperty(t => t.LastError, "Send timeout (no ACK received)")
                    .SetProperty(t => t.NextAttemptAt, now));
        }

        /// <summary>
        /// When a brain (re)connects, expedite all its waiting tasks by resetting NextAttemptAt to now.
        /// Returns the number of tasks flushed.
        /// </summary>
        public async Task<int> FlushBrainAsync(Guid accountUid, string brainUid)
        {
            var now = DateTime.UtcNow;

            return await Db.SyncTasks
                .Where(t => (t.Status == (byte)SyncTaskStatus.Pending
                          || t.Status == (byte)SyncTaskStatus.Failed)
                         && t.AccountUid == accountUid
                         && t.BrainUid == brainUid)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.NextAttemptAt, now));
        }

        /// <summary>
        /// Count tasks still waiting (Pending/Failed) for a brain — used to decide when to coalesce a
        /// large delta backlog into a single SynchronizeUsers snapshot (Task #8).
        /// </summary>
        public async Task<int> GetPendingCountAsync(Guid accountUid, string brainUid)
        {
            return await _repo.CountAsync(t =>
                t.AccountUid == accountUid &&
                t.BrainUid == brainUid &&
                (t.Status == (byte)SyncTaskStatus.Pending || t.Status == (byte)SyncTaskStatus.Failed));
        }

        private static string? Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
                return value;

            return value[..maxLength];
        }
    }
}
