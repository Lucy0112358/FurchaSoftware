namespace FurchaAdminApi.Models.Result
{
    public class AllModulesResult
    {
        public string BranchName { get; set; }

        public List<BranchModules> Modules { get; set; }

    }

    public class BranchModules
    {
        public int Id { get; set; }

        public string? LockerRange { get; set; }
        public string? Info { get; set; }
        public string? GroupName { get; set; }

        public int? AcessControl { get; set; } = null;

        public int? AlarmSystem { get; set; } = null;
    }
}