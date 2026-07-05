using FurchaBLL.Constants;
using FurchaDAL.Models;
using FurchaDAL.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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

        private const int MaxAttempts = 8;

        public SyncTaskService(BaseRepository<SyncTask> repo)
        {
            _repo = repo;
        }

        public static SyncTask BuildSyncTask(SyncTaskStatus status,
            Guid accountUid, string brainUid, CommandTypes commandType,
            int entityId, string entityType, SyncTaskOperationType operation,
            object envelopForMqtt)
        {
            return new SyncTask
            {
                Status = (byte)status,
                Attempts = 0,
                MaxAttempts = MaxAttempts,
                AccountUid = accountUid,
                BrainUid = brainUid,
                CommandType = (int)commandType,
                EntityId = entityId, // User.Id or UserGroup.Id
                EntityType = entityType, // "User" or "UserGroup"
                LastError = null,
                Operation = (byte)operation, // 1=Upsert, 2=Delete
                Payload = JsonSerializer.Serialize(envelopForMqtt),
                SentAt = null,
                AckedAt = null,
                NextAttemptAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
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
            if (Db.Database.CurrentTransaction is not null)
            {
                // Enlist in the caller's transaction (e.g. an API hook writing the data change
                // and this task atomically). No new transaction here.
                await CoalesceAndInsertAsync(task);
                return task;
            }

            // Standalone call: open our own transaction via the configured execution strategy.
            // EnableRetryOnFailure forbids a bare BeginTransaction, so it must go through the strategy.
            var strategy = Db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await Db.Database.BeginTransactionAsync();
                await CoalesceAndInsertAsync(task);
                await tx.CommitAsync();
            });

            return task;
        }

        public async Task<SyncTask?> ClaimNextAsync()
        {
            var now = DateTime.UtcNow;

            var task = await Db.SyncTasks
                .Where(t => (t.Status == (byte)SyncTaskStatus.Pending
                          || t.Status == (byte)SyncTaskStatus.Failed)
                         && t.Attempts < t.MaxAttempts
                         && t.NextAttemptAt <= now
                         && Db.BrainModules.Any(b => b.BrainUid == t.BrainUid && b.IsOnline)
                )
                .OrderBy(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            if (task == null)
                return null;

            var isUpdated = await Db.SyncTasks
                .Where(t => t.Id == task.Id
                         && (t.Status == (byte)SyncTaskStatus.Pending ||
                            t.Status == (byte)SyncTaskStatus.Failed)
                        && t.NextAttemptAt <= now)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.NextAttemptAt, now.AddMinutes(1)));

            if (isUpdated == 0)
                return null;

            await Db.Entry(task).ReloadAsync();

            return task;
        }

        public async Task<bool> MarkFailedAsync(SyncTask task, string errorMessage)
        {
            var updatesCount = await Db.SyncTasks
                .Where(t => t.Id == task.Id
                         && (t.Status == (byte)SyncTaskStatus.Pending ||
                            t.Status == (byte)SyncTaskStatus.Failed))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Status, t => t.Attempts + 1 >= t.MaxAttempts ?
                        (byte)SyncTaskStatus.Dead : (byte)SyncTaskStatus.Failed)
                    .SetProperty(t => t.Attempts, t => t.Attempts + 1)
                    .SetProperty(t => t.NextAttemptAt, DateTime.UtcNow.AddMinutes(5))
                    .SetProperty(t => t.LastError, errorMessage));

            return updatesCount > 0;
        }

        public async Task<bool> MarkAckedAsync(long taskId)
        {
            var updatesCount = await Db.SyncTasks
                .Where(t => t.Id == taskId
                         && t.Status == (byte)SyncTaskStatus.Sent)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Status, (byte)SyncTaskStatus.Acked)
                    .SetProperty(t => t.AckedAt, DateTime.UtcNow));

            return updatesCount > 0;
        }

        public async Task<bool> MarkSentAsync(SyncTask task)
        {
            var updatesCount = await Db.SyncTasks
                .Where(t => t.Id == task.Id
                         && (t.Status == (byte)SyncTaskStatus.Pending ||
                            t.Status == (byte)SyncTaskStatus.Failed))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.Status, (byte)SyncTaskStatus.Sent)
                    .SetProperty(t => t.SentAt, DateTime.UtcNow)
                    .SetProperty(t => t.Attempts, t => t.Attempts + 1));

            return updatesCount > 0;
        }

        private async Task CoalesceAndInsertAsync(SyncTask task)
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
        }
    }
}
