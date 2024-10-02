namespace Domain.Entities
{
    public class UserGroup_Branch
    {
        // in a service method where it is used check that company is the same for ug and b
        public int Id { get; set; }
        public int UserGroupId { get; set; }
        public int BranchId { get; set; }
    }
}