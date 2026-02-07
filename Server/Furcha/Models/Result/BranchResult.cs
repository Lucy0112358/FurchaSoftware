namespace FurchaAdminApi.Models.Result
{
    public class BranchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> Lockers { get; set; }
        public IEnumerable<LockerGroupResult> LockerGroups { get; set; }
    }
}
