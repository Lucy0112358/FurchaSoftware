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

            /* SERVICES */
            services.AddScoped(typeof(UserService));

            return services;
        }
    }
}
