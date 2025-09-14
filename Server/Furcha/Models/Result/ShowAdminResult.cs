namespace FurchaAdminApi.Models.Result
{
    public class ShowAdminResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Role { get; set; }

        public int? RoleId { get; set; }

        public List<PermissionResult> Permissions { get; set; }

        public List<AdminBranchResult> Branches { get; set; }

        public bool IsActive { get; set; }
    }
}


