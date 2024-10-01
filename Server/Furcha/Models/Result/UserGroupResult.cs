namespace FurchaAdminApi.Models.Result
{
    public class UserGroupResult
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<LockerGroupResult> PermittedLockers { get; set; } 
        public List<string> BranchNames { get; set; } 
        public string State { get; set; }
    }
}
