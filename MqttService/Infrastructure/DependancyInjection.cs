using MqttService.Application.Repositories;
using MqttService.Application.Services;

namespace MqttService.Infrastructure
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddInfrasructure(this IServiceCollection services)
        {
            /* REPOSITORIES */
            services.AddScoped(typeof(CardLockerRepository));

            /* SERVICES */
            services.AddScoped(typeof(CardLockerService));

            return services;
        }
    }
}
