namespace FurchaAdminApi.Models.Request
{
    public class UserGroupRequest
    {
        public string Name { get; set; }
        public int AdminId { get; set; }
        public List<int> Branches
        {
            get; set;
        }
    }
}
