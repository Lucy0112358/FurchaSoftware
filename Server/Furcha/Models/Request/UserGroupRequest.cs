namespace FurchaAdminApi.Models.Request
{
    public class UserGroupRequest
    {
        public string UserGroupName { get; set; }
        public List<int> Branches
        {
            get; set;
        }
    }
}
