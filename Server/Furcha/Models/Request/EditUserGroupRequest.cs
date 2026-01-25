namespace FurchaAdminApi.Models.Request
{
    public class EditUserGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> LockerIds { get; set; }
    }
}
