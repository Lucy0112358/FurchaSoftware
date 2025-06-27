using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;
using BLL.Services;

namespace FurchaJobService.Workers
{
    public class MqttMainWorker : BackgroundService
    {
        private readonly ILogger<MqttMainWorker> _logger;
        private readonly MqttService _mqttService;

        public MqttMainWorker(ILogger<MqttMainWorker> logger, MqttService mqttService)
        {
            _logger = logger;
            _mqttService = mqttService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               await _mqttService.InitializeClient();
            }
        }

    }
}
