using Domain.Configuration;
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
            services.AddScoped(typeof(LockerRepository));


            /* SERVICES */
            services.AddScoped(typeof(UserService));
            services.AddScoped(typeof(AuthenticationService));
            services.AddScoped(typeof(LockerService));
            services.AddScoped(typeof(ISanitizer), typeof(Sanitizer));

            return services;
        }
    }
}
