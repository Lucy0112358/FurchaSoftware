using MqttService.Application.Repositories;
using MqttService.Infrastructure.Services;

namespace MqttService.Infrastructure
{
    public static class MqttServiceInjection
    {
        public static IServiceCollection AddInfrasructure(this IServiceCollection services)
        {
            /* REPOSITORIES */
            services.AddScoped(typeof(CardLockerRepository));

            /* SERVICES */
            services.AddScoped(typeof(CardLockerService));
            services.AddScoped(typeof(MqttClientService));

            return services;
        }
    }
}
