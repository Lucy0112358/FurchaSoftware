namespace FurchaAdminApi.Models.Request
{
    public class EditLockerRequest
    {
        public int? GroupId { get; set; }
        public List<int> LockerIds { get; set; } = new();
        public string? Type { get; set; }
    }

}
