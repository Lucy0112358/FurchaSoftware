namespace FurchaAdminApi.Models.Request
{
    public class ChangeUsersGroupRequest
    {
        public List<int> Ids { get; set; }

        public int GroupId { get; set; }
    }
}
