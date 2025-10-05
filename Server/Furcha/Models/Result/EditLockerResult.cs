namespace FurchaAdminApi.Models.Result
{
    public class EditLockerResult
    {
        public List<int> SuccessfulGroups { get; set; } = new();
        public List<(int GroupId, string Error)> FailedGroups { get; set; } = new();
    }
}
