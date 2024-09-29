using FurchaAdminApi.Repos;
using FurchaAdminApi.Services;

namespace MqttService.Infrastructure
{
    public static class AdminServiceInjection
    {
        public static IServiceCollection GenerateInjectionAdmin(this IServiceCollection services)
        {
            /* REPOSITORIES */
            services.AddScoped(typeof(UserRepository));
            services.AddScoped(typeof(BranchRepository));
            services.AddScoped(typeof(AdminRepository));
            

            /* SERVICES */
            services.AddScoped(typeof(UserService));
            services.AddScoped(typeof(AuthenticationService));

            return services;
        }
    }
}
