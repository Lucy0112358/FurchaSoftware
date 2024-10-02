namespace FurchaAdminApi.Models.Result
{
    public class LockerGroupResult
    {
        public string LockerGroupName { get; set; }
        public List<PermittedLockerResult> LockersFromGroup { get; set; }
    }

    public class PermittedLockerResult
    {
        public int LockerId { get; set; }
        public long LockerNumber { get; set; }
    }
}
