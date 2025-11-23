using Domain.Enums;

namespace FurchaAdminApi.Models.Result
{
    public class UserResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? ActiveTo { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public string Phone { get; set; }

        //here we only send the name of the role, as frontend only needs that information to display in the UI
        public string Role { get; set; }
        public int State { get; set; }
        public List<CardResult> Cards { get; set; }
        public List<UserGroupResult> UserGroups { get; set; }
        public List<BranchResult> Branches { get; set; }
    }
}
