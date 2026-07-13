using FurchaBLL.Interfaces;
using FurchaBLL.Services;
using FurchaDAL.Models;
using FurchaDAL.Repositories;
using FurchaJobService.Workers;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;

namespace FurchaJobService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var exeDir = System.IO.Path.GetDirectoryName(exePath);
            System.IO.Directory.SetCurrentDirectory(exeDir);
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddDbContextFactory<furchaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
            builder.Services.AddSingleton<IMqttClient>(new MqttFactory().CreateMqttClient());
            builder.Services.AddSingleton<MqttClientOptions>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>().GetSection("MqttSettings");
                var logger = sp.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("MQTT Configuration: Server={Server}, Port={Port}, ClientId={ClientId}",
                    config["Server"], config["Port"], config["ClientId"]);
                return new MqttClientOptionsBuilder()
                    .WithClientId(config["ClientId"])
                    .WithTcpServer(config["Server"], int.Parse(config["Port"]))
                    .WithCredentials(config["Username"], config["Password"])
                    .WithCleanSession()
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                    .WithWillTopic("server/status/will")
                    .WithWillPayload("{\"Status\":\"Offline\"}")
                    .WithWillRetain(true)
                    .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();
            });

            builder.Services.AddSingleton<IMqttService, MqttService>();
            builder.Services.AddSingleton<IMqttMessageHandler, MqttMessageHandler>();
            builder.Services.AddSingleton<IApiSocketClient, ApiSocketClient>();
            builder.Services.AddScoped(typeof(BaseRepository<>));
            builder.Services.AddScoped<SyncTaskService>();
            builder.Services.AddHostedService<MqttBackgroundService>();
            builder.Services.AddHostedService<SyncTaskWorkerService>();
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "FurchaJobService";
            });
            var host = builder.Build();
            host.Run();
        }
    }
}
