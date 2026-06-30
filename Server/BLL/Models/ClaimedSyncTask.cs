namespace FurchaBLL.Models
{
    /// <summary>
    /// Projection returned by the atomic "claim next task" query.
    /// Column names must match the OUTPUT clause in SyncTaskService.ClaimNextAsync.
    /// </summary>
    public class ClaimedSyncTask
    {
        public long Id { get; set; }
        public Guid AccountUid { get; set; }
        public string BrainUid { get; set; } = string.Empty;
        public int CommandType { get; set; }
        public byte Operation { get; set; }
        public string Payload { get; set; } = string.Empty;
    }
}
