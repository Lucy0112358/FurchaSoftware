namespace FurchaAdminApi.Models.Request
{
    public class EditUserGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminId { get; set; }
        public List<int> Branches { get; set; } = new();
        public List<int> LockerIds { get; set; } = new();
    }
}
