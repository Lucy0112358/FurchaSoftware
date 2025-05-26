using Domain.Entities;

namespace FurchaAdminApi.Models.Result
{
    public class PermissionResult
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public List<RolePermissionResult> Permissions { get; set; } 
    }

}
