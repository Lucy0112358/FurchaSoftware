using FurchaBLL.Constants;
using FurchaBLL.MqttModels.Publish;
using FurchaBLL.MqttModels.Subscribe;
using FurchaBLL.MqttModels;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using System.Text.Json;
using System.Text;
using FurchaDAL.Models;

public class MqttService
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

    public async Task InitializeClient(CancellationToken stoppingToken)
    {
        _mqttClient.ConnectedAsync += async e =>
        {
            _logger.LogInformation("Connected to MQTT broker. Subscribing to topics...");

            await _mqttClient.SubscribeAsync("$CONTROL/dynamic-security/#");
            await _mqttClient.SubscribeAsync("$SYS/broker/clients/connected");
            await _mqttClient.SubscribeAsync("webserver/#");
            await _mqttClient.SubscribeAsync("server/status/will");

            var topic = "controller/status/will";
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload("{\"Status\":\"Online\"}")
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(mqttMessage);
        };

        _mqttClient.DisconnectedAsync += async e =>
        {
            if (stoppingToken.IsCancellationRequested) return;

            _logger.LogWarning("MQTT disconnected. Reason: {Reason}", e.Reason);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reconnection attempt failed.");
            }
        };

        _mqttClient.ApplicationMessageReceivedAsync += HandleRequest;

        await ConnectWithRetry(stoppingToken);
    }

    private async Task ConnectWithRetry(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = await _mqttClient.ConnectAsync(_mqttOptions, stoppingToken);
                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    _logger.LogInformation("MQTT connected successfully.");
                    break;
                }

                _logger.LogWarning("MQTT connection failed: {Result}. Retrying...", result.ResultCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT connection failed. Retrying in 5 seconds...");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task HandleRequest(MqttApplicationMessageReceivedEventArgs e)
  {
        var topic = e.ApplicationMessage.Topic;
        var responseMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        _logger.LogInformation("Received MQTT message on topic {Topic}", topic);

        try
        {    

            if (topic.StartsWith("webserver/"))
            {
                var message = JsonSerializer.Deserialize<MqttBaseRequest<object>>(responseMessage);
                switch ((CommandTypes)message.Command)
                {
                    case CommandTypes.OpenLocker:
                        var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<object>>(responseMessage);
                        // db update logic
                        break;

                    case CommandTypes.OpenLockersFromAdmin:
                        var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<int>>>(responseMessage);
                        // process locker IDs
                        break;

                    case CommandTypes.CreateUserFromAdmin:
                        var userPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<Locker>>>(responseMessage);
                        // handle user creation
                        break;
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogInformation("Raw MQTT payload (topic: {Topic}): {Payload}",
                       e.ApplicationMessage.Topic,
                       Encoding.UTF8.GetString(e.ApplicationMessage.Payload));

            _logger.LogError(ex, "Failed to deserialize MQTT payload from topic {Topic}: {Payload}", topic, responseMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MQTT message from topic {Topic}", topic);
        }
    }

    public async Task AddAccount(Guid? accountUID, string brainPass)
    {
        var mqttCompany = new MqttBaseRequest<MqttCreateCompany>
        {
            Command = (int)CommandTypes.CreateAccount,
            Data = new MqttCreateCompany
            {
                Username = accountUID.ToString(),
                Password = brainPass,
                Roles = new List<MqttRole>
                {
                    new MqttRole { RoleName = "user", Priority = 1 }
                }
            },
            ReceivedDate = DateTime.UtcNow
        };

        await PublishToMqtt(mqttCompany, "$CONTROL/dynamic-security/v1");
    }

    public async Task PublishToMqtt<T>(MqttBaseRequest<T> request, string topic)
    {
        try
        {
            if (!_mqttClient.IsConnected)
                await _mqttClient.ConnectAsync(_mqttOptions);

            var payload = JsonSerializer.Serialize(request);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag(true)
                .Build();

            await _mqttClient.PublishAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish MQTT message to topic {Topic}", topic);
        }
    }
}
