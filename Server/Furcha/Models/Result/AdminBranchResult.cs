namespace FurchaAdminApi.Models.Result
{
    public class AdminBranchResult
    {
        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public List<int> GroupIds { get; set; }
    }
}
