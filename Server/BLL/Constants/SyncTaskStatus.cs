namespace FurchaBLL.Constants
{
    /// <summary>
    /// Lifecycle of a row in furcha.SyncTask (the cloud→brain MQTT outbox).
    /// Values MUST match the TINYINT stored in the table / CK_SyncTask_Status.
    /// </summary>
    public enum SyncTaskStatus : byte
    {
        Pending = 0,     // created, waiting to be sent
        Sent = 1,        // published, waiting for the brain's ACK
        Acked = 2,       // confirmed by the brain (success)
        Failed = 3,      // will retry (with backoff)
        Dead = 4,        // gave up after MaxAttempts (raise alert)
        Superseded = 5   // coalesced — a newer task replaced it
    }
}
