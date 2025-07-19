using BLL.Services;
using FurchaDAL.Models;
using FurchaJobService.Workers;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using Microsoft.Extensions.Options;
using FurchaBLL.Models;

namespace FurchaJobService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.Configure<MqttSettings>(
                builder.Configuration.GetSection("Mqtt"));

            builder.Services.AddDbContextFactory<furchaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

            builder.Services.AddSingleton<IMqttClient>(sp =>
            {
                var factory = new MqttFactory();
                return factory.CreateMqttClient();
            });

            builder.Services.AddSingleton<MqttClientOptions>(sp =>
            {
                var mqttSettings = sp.GetRequiredService<IOptions<MqttSettings>>().Value;

                return new MqttClientOptionsBuilder()
                .WithClientId(mqttSettings.ClientId)
                    .WithTcpServer(mqttSettings.Host, mqttSettings.Port)
                    .WithCredentials(mqttSettings.Username, mqttSettings.Password)
                    .WithCleanSession()
                    .Build();
            });

            builder.Services.AddSingleton<MqttService>();
            builder.Services.AddHostedService<MqttMainWorker>();

            var host = builder.Build();
            host.Run();
        }
    }
}
