using Domain.Entities;
using FurchaAdminApi.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace FurchaAdminApi.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<IEnumerable<Permission>> GetPermissionsByAdminId(int adminId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
                return Task.FromResult(Enumerable.Empty<Permission>());

            var permissionsJson = user.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsJson))
                return Task.FromResult(Enumerable.Empty<Permission>());

            var permissionNames = JsonSerializer.Deserialize<List<string>>(permissionsJson);

            var permissions = permissionNames.Select((name, index) => new Permission
            {
                Id = index + 1, // Optional: Adjust as needed
                Name = name,
                Description = string.Empty
            });

            return Task.FromResult<IEnumerable<Permission>>(permissions);
        }
    }
}
