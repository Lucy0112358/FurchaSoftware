namespace FurchaAdminApi.Models.Request
{
    public class SetUserRequest
    {
        public List<int> LockerIds { get; set; }
        public int UserId { get; set; }
    }
}
