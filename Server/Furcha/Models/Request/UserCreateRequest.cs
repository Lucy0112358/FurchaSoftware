using Domain.Entities;

namespace FurchaAdminApi.Models.Request
{
    public class UserCreateRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        //todo improve phone number
        public string Phone { get; set; }

        public bool IsPinRequired { get; set; }

        public DateTime? ActiveFrom { get; set; }

        public DateTime? ActiveTo { get; set; }

        public List<int> UserGroups { get; set; }

        public List<int> LockerIds { get; set; }
        public List<string> Cards { get; set; }

        //this property is to be deleted as soon as auth is done
    //    public int adminId { get; set; }
    }
}
