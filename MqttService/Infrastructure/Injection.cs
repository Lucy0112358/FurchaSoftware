using MqttService.Repositories;

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
        }
    }
}
