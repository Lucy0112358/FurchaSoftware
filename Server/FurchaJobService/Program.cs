using FurchaDAL.Models;
using FurchaJobService.Workers;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using MQTTnet.Client;
using FurchaBLL.Models;

namespace FurchaJobService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);        

            builder.Services.AddDbContextFactory<furchaContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
            builder.Services.AddSingleton<IMqttClient>(new MqttFactory().CreateMqttClient());
            builder.Services.AddSingleton<MqttClientOptions>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>().GetSection("MqttSettings");
                return new MqttClientOptionsBuilder()
                    .WithClientId(config["ClientId"])
                    .WithTcpServer(config["Server"], int.Parse(config["Port"]))
                    .WithCredentials(config["Username"], config["Password"])
                    .Build();
            });

            builder.Services.AddSingleton<MqttService>();
            builder.Services.AddHostedService<MqttMainWorker>();
            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Furcha Job Service"; 
            });
            var host = builder.Build();
            host.Run();
        }
    }
}
