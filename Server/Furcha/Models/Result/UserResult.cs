using Domain.Entities;
using Domain.Enums;

namespace FurchaAdminApi.Models.Result
{
    public class UserResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public RoleEnum Role { get; set; }
        public StateEnum State { get; set; }
        public List<CardResult> Cards { get; set; }
        public List<UserGroupResult> UserGroups { get; set; }
        public List<BranchResult> Branches { get; set; }
    }
}
