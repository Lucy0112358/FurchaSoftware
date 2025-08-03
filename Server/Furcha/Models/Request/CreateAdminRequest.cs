namespace FurchaAdminApi.Models.Request
{
    public class CreateAdminRequest
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

/*        public string PasswordHash { get; set; } = "b33f9a399e11b3c36f3d3b338668c41214d8256a95594664166af9fb8b9b72d5";

        public string Salt { get; set; } = "furcha-salt";

        public int CompanyId { get; set; }*/

        public List<int> Permissions { get; set; }

        public List<int>? LockerIds { get; set; }

        public List<int>? Branches { get; set; }

        public int? ModifiedBy { get; set; }
    }
}
