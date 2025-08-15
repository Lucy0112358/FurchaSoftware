namespace FurchaAdminApi.Models.Result
{
    public class RolePermissionResult
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsOptional { get; set; }
        public int? ObjectTypeId { get; set; }
    }

}
