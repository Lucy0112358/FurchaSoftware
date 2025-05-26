namespace FurchaAdminApi.Models.Request
{
    public class EditLockerRequest
    {
        public List<int> LockerIds { get; set; } = new();
        public string? Type { get; set; }
    }

}
