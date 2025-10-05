namespace FurchaAdminApi.Models.Result
{
    public class LockersResult
    {
        public string GroupName { get; set; }
        public List<LockerWithUsers> GroupLockers { get; set; }
    }

    public class LockerWithUsers
    {
        public int Id { get; set; }

        public long number { get; set; }

        public int? groupid { get; set; }

        public int? LockerType { get; set; }

        public int IsActive { get; set; }

        public int IsOpen { get; set; }

        public int BranchId { get; set; }

        public string PasswordHash { get; set; }

        public List<string> Users { get; set; }
    }
}