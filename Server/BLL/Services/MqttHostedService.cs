using FurchaBLL.Interfaces;
using Microsoft.Extensions.Hosting;

namespace FurchaBLL.Services
{
    public class MqttHostedService : BackgroundService
    {
        private readonly IMqttApiService _mqttService;

        public MqttHostedService(IMqttApiService mqttService)
        {
            _mqttService = mqttService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttService.IsConnected)
                    {
                        await _mqttService.ConnectAsync();
                    }
                }
                catch (Exception ex)
                {
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); 
            }
        }
    }
}
