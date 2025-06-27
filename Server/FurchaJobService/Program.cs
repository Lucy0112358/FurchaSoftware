using BLL.Services;
using FurchaDAL.Models;
using FurchaJobService.Workers;
using Microsoft.EntityFrameworkCore;

namespace FurchaJobService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddDbContextFactory<furchaContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

            builder.Services.AddHostedService<MqttMainWorker>();
            builder.Services.AddSingleton<MqttService>();

            var host = builder.Build();
            host.Run();
        }
    }
}