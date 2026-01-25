
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
                await _mqttService.InitializeClient(stoppingToken);

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
