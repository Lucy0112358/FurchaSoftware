using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models.Security;

namespace MqttServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                           .ConfigureServices((context, services) =>
                           {
                               // Add application services here
                               services.AddTransient<ISomeService, SomeService>();
                               services.AddTransient<App>();
                           })
                           .Build();

            // Resolve and run the application entry point
            var app = host.Services.GetRequiredService<App>();
            await app.RunAsync();
        }
    }
}
