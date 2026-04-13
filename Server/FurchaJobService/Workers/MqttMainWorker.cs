
using FurchaBLL.Interfaces;

namespace FurchaJobService.Workers
{
    /// <summary>
    /// DEPRECATED: Use MqttBackgroundService instead.
    /// Kept for backward compatibility during migration.
    /// </summary>
    [Obsolete("Use MqttBackgroundService instead.", false)]
    public class MqttMainWorker : BackgroundService
    {
        private readonly ILogger<MqttMainWorker> _logger;
        private readonly IMqttService _mqttService;

        public MqttMainWorker(ILogger<MqttMainWorker> logger, IMqttService mqttService)
        {
            _logger = logger;
            _mqttService = mqttService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("MQTT client initialized.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("MQTT worker canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT worker failed.");
                throw;
            }
        }
    }
}
