using Domain.Entities;
using FurchaAdminApi.Middlewares;

namespace FurchaAdminApi.Services
{
    public class PermissionService : IPermissionService
    {
        public Task<IEnumerable<Permission>> GetPermissionsByAdminId(int adminId)
        {
            var permissions = new List<Permission>
    {
        new Permission { Id = 1, Name = "ManageUsers", Description = "Can manage users" },
        new Permission { Id = 2, Name = "ViewReports", Description = "Can view reports" }
    };

            return Task.FromResult<IEnumerable<Permission>>(permissions);
        }

    }
}
