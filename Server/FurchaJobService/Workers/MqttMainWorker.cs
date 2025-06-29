using FurchaDAL.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
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
            try
            {
                await _mqttService.InitializeClient();
                // Optionally log that the client was initialized successfully.
                _logger.LogInformation("MQTT client initialized.");

                // Keep the service alive until cancellation
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Expected on shutdown
                _logger.LogInformation("MQTT worker canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT worker failed.");
                throw; // Optional: rethrow to crash the service or handle otherwise
            }
        }
    }
}
