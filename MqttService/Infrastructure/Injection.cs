using MqttService.Repositories;
using MqttService.Services;

namespace MqttService.Infrastructure
{
    public class Injection
    {
        public void GenerateInjectionFurcha(IServiceCollection services)
        {
            GenerateInjection(services);
        }

        private static void GenerateInjection(IServiceCollection services)
        {
            /* REPOSITORIES */
            services.AddScoped(typeof(CardLockerRepository));

            /* SERVICES */
            services.AddScoped(typeof(CardLockerService));            
        }
    }
}
