using Domain.Configuration;
using FurchaAdminApi.Middlewares;
using FurchaAdminApi.Repos;
using FurchaAdminApi.Services;
using FurchaBLL.Services;
using FurchaDAL.Repositories;

namespace FurchaAdminApi.Infrustructures
{
    public static class AdminServiceInjection
    {
        public static IServiceCollection GenerateInjectionAdmin(this IServiceCollection services)
        {
            /* REPOSITORIES */
            services.AddScoped(typeof(BaseRepository<>));
            services.AddScoped(typeof(UserRepository));
            services.AddScoped(typeof(BranchRepository));
            services.AddScoped(typeof(AdminRepository));
            services.AddScoped(typeof(LockerRepository));
            services.AddScoped(typeof(BranchService));

            /* SERVICES */
            services.AddScoped(typeof(SyncTaskService));
            services.AddScoped(typeof(UserService));
            services.AddScoped(typeof(AuthenticationService));
            services.AddScoped(typeof(LockerService));
            services.AddScoped(typeof(ISanitizer), typeof(Sanitizer));
            services.AddScoped(typeof(IPermissionService), typeof(PermissionService));
            //services.AddSingleton<IMqttService, MqttService>();

            return services;
        }
    }
}
