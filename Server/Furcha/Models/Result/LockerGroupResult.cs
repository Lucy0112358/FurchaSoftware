namespace FurchaAdminApi.Models.Result
{
    public class LockerGroupResult
    {
        public string Name { get; set; }
        public List<PermittedLockerResult> permittedLockerResults { get; set; }
    }

    public class PermittedLockerResult
    {
        public int LockerId { get; set; }
        public long LockerNumber { get; set; }
    }
}
