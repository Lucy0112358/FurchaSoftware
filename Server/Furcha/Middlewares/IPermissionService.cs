using Domain.Entities;

namespace FurchaAdminApi.Middlewares
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetPermissionsByAdminId(int adminId);
    }
}
