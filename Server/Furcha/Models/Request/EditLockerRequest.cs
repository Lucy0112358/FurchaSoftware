namespace FurchaAdminApi.Models.Request
{
    public class EditLockerRequest
    {
        public List<int> LockerIds { get; set; } = new();
        public int Type { get; set; }
    }

}
