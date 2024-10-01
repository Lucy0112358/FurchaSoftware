namespace Domain.Entities
{
    public class UserGroup_Branch
    {
        public int Id { get; set; }
        public int UserGroupId { get; set; }
        public int BranchId { get; set; }
    }
}