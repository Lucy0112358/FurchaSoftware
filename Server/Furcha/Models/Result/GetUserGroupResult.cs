namespace FurchaAdminApi.Models.Result
{
    public class GetUserGroupResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserGroupsBranch> Branches { get; set; }
    }

    public class UserGroupsBranch
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public List<int> Lockers { get; set; }
    }

}
