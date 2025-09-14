namespace FurchaAdminApi.Models.Result
{
    public class AdminBranchResult
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public List<LockerResult> Lockers { get; set; }
    }
}
