using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;

namespace FurchaBLL.Services
{
    /// <summary>
    /// Service for handling MQTT client operations including publishing messages and managing connections.
    /// </summary>
    public class MqttService : IMqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private readonly ILogger<MqttService> _logger;

        public MqttService(IMqttClient mqttClient, MqttClientOptions mqttOptions, ILogger<MqttService> logger)
        {
            _mqttClient = mqttClient;
            _mqttOptions = mqttOptions;
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsConnected => _mqttClient.IsConnected;

        /// <inheritdoc />
        public void RegisterMessageHandler(Func<MqttApplicationMessageReceivedEventArgs, Task> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _mqttClient.ApplicationMessageReceivedAsync += handler;
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            var maxRetries = 10;
            while (maxRetries > 0)
            {
                try
                {
                    var result = await _mqttClient.ConnectAsync(_mqttOptions);
                    if (result.ResultCode == MqttClientConnectResultCode.Success)
                    {
                        _logger.LogInformation("MQTT connected successfully.");
                        return;
                    }

                    _logger.LogWarning("MQTT connection failed: {Result}. Retrying...", result.ResultCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MQTT connection failed. Retrying in 5 seconds...");
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
                maxRetries = maxRetries - 1;
            }
        }

        private async Task EnsureConnectedAsync()
        {
            if (!_mqttClient.IsConnected)
            {
                await ConnectAsync();
            }
        }

        /// <inheritdoc />
        public async Task PublishAsync<T>(MqttBaseRequest<T> request, string topic)
        {
            try
            {
                await EnsureConnectedAsync();

                var payload = JsonSerializer.Serialize(request);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag(false)
                    .Build();

                await _mqttClient.PublishAsync(message);
            }
            catch (MQTTnet.Exceptions.MqttClientNotConnectedException)
            {
                _logger.LogWarning("MQTT disconnected while publishing to {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish MQTT message to topic {Topic}", topic);
            }
        }

        public async Task<MqttClientPublishResult> PublishAsync(object request, string topic, bool withRetainFlag,
            MQTTnet.Protocol.MqttQualityOfServiceLevel qos = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
        {
            try
            {
                await EnsureConnectedAsync();

                var payload = JsonSerializer.Serialize(request);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(payload)
                    .WithRetainFlag(withRetainFlag)
                    .WithQualityOfServiceLevel(qos)
                    .Build();

                return await _mqttClient.PublishAsync(message);
            }
            catch (MQTTnet.Exceptions.MqttClientNotConnectedException)
            {
                _logger.LogWarning("MQTT disconnected while publishing to {Topic}", topic);

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish MQTT message to topic {Topic}", topic);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task AddAccountAsync(Guid? accountUID, string brainPass)
        {
            await EnsureConnectedAsync();

            var commandWrapper = new
            {
                commands = new object[]
                {
                    new
                    {
                        Command = "createClient",
                        Username = accountUID,
                        Password = brainPass,
                        ClientId = "",
                        TextName = "",
                        TextDescription = "",
                        Roles = new[]
                        {
                            new { RoleName = "user", Priority = 1 }
                        }
                    }
                }
            };

            var payload = JsonSerializer.Serialize(commandWrapper);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("$CONTROL/dynamic-security/v1")
                .WithPayload(payload)
                .WithRetainFlag(false)
                .Build();

            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SubscribeAsync(params string[] topics)
        {
            await EnsureConnectedAsync();

            if (topics == null || topics.Length == 0)
                throw new ArgumentException("At least one topic must be provided.", nameof(topics));

            foreach (var topic in topics)
            {
                await _mqttClient.SubscribeAsync(topic);
            }
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
    }
}
