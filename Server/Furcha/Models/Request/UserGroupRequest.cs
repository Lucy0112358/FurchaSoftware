namespace FurchaAdminApi.Models.Request
{
    public class UserGroupRequest
    {
        public string Name { get; set; }
        public int adminId { get; set; }
        public List<int> Branches
        {
            get; set;
        }
    }
}
