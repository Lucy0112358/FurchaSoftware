using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;

namespace FurchaJobService.Workers
{
    /// <summary>
    /// Background service that manages the MQTT client lifecycle and initialization.
    /// Handles connection, subscription to topics, and graceful shutdown.
    /// </summary>
    public class MqttBackgroundService : BackgroundService
    {
        private readonly ILogger<MqttBackgroundService> _logger;
        private readonly IMqttService _mqttService;
        private readonly IMqttMessageHandler _messageHandler;
        private readonly string[] _subscribedTopics =
        [
            "$CONTROL/dynamic-security/#",
            "$SYS/broker/clients/connected",
            "webserver/#",
            "server/status/will"
        ];

        public MqttBackgroundService(
            ILogger<MqttBackgroundService> logger,
            IMqttService mqttService,
            IMqttMessageHandler messageHandler)
        {
            _logger = logger;
            _mqttService = mqttService;
            _messageHandler = messageHandler;
        }

        /// <summary>
        /// Executes the background service logic.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token for service shutdown.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting MQTT background service.");

                _mqttService.RegisterMessageHandler(_messageHandler.HandleAsync);

                await InitializeConnectionAsync(stoppingToken);
                await HandleConnectionAsync(stoppingToken);

                _logger.LogInformation("MQTT service initialized and running.");

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("MQTT background service canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT background service encountered an error.");
                throw;
            }
        }

        /// <summary>
        /// Initializes the MQTT connection and subscribes to configured topics.
        /// </summary>
        private async Task InitializeConnectionAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Connecting to MQTT broker.");
                await _mqttService.ConnectAsync();

                _logger.LogInformation("Subscribing to MQTT topics.");
                await _mqttService.SubscribeAsync(_subscribedTopics);

                _logger.LogInformation("Subscribed to topics: {Topics}", string.Join(", ", _subscribedTopics));

                await PublishOnlineStatusAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MQTT initialization.");
                throw;
            }
        }

        /// <summary>
        /// Handles the MQTT connection state and implements reconnection logic.
        /// </summary>
        private async Task HandleConnectionAsync(CancellationToken stoppingToken)
        {
            const int reconnectDelay = 5;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!_mqttService.IsConnected)
                    {
                        _logger.LogWarning("MQTT connection lost. Attempting to reconnect.");
                        await Task.Delay(TimeSpan.FromSeconds(reconnectDelay), stoppingToken);
                        await _mqttService.ConnectAsync();
                        await _mqttService.SubscribeAsync(_subscribedTopics);
                        await PublishOnlineStatusAsync();
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during connection handling. Retrying in {Delay} seconds.", reconnectDelay);
                    await Task.Delay(TimeSpan.FromSeconds(reconnectDelay), stoppingToken);
                }
            }
        }

        /// <summary>
        /// Publishes an online status message to signal the service is ready.
        /// </summary>
        private async Task PublishOnlineStatusAsync()
        {
            try
            {
                await _mqttService.PublishAsync<object>(
                    new MqttBaseRequest<object>
                    {
                        Command = 0,
                        Operation = 0,
                        Data = null
                    },
                    "controller/status/will");

                _logger.LogInformation("Published online status message.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing online status message.");
            }
        }

        /// <summary>
        /// Cleanly shuts down the MQTT service.
        /// </summary>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping MQTT background service.");

            try
            {
                await _mqttService.DisconnectAsync();
                _logger.LogInformation("MQTT client disconnected.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MQTT disconnection.");
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
