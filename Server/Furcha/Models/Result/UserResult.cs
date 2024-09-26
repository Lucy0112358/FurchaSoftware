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
        public List<Card> Cards { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<Branch> Branches { get; set; }
    }
}
