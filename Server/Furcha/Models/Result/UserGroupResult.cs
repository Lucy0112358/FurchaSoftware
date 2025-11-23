namespace FurchaAdminApi.Models.Result
{
    public class UserGroupResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<LockerGroupResult> PermittedLockers { get; set; } 
        public List<string> BranchNames { get; set; } 
        public int State { get; set; }
        public int UserCount { get; set; }
    }
}
