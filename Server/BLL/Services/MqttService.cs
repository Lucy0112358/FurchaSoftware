using FurchaBLL.Constants;
using FurchaBLL.MqttModels.Subscribe;
using FurchaBLL.MqttModels;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using System.Text.Json;
using System.Text;
using FurchaDAL.Models;
using FurchaBLL.MqttModels.Publish;
using Microsoft.Extensions.DependencyInjection;

public class MqttService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly ILogger<MqttService> _logger;
    // private readonly furchaContext Db;
    private readonly IServiceScopeFactory _scopeFactory;

    public MqttService(IMqttClient mqttClient, MqttClientOptions mqttOptions, ILogger<MqttService> logger, /*furchaContext db,*/ IServiceScopeFactory scopeFactory)
    {
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;
        _logger = logger;
        // Db = db;
        _scopeFactory = scopeFactory;
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
                        using (var Db = furchaContext.Create())
                        {
                            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<OpenLockerRequest>>(responseMessage);
                            var dbLocker = Db.Lockers.Where(x => x.Id == lockerPayload.Data.LockerId).FirstOrDefault();
                            dbLocker.LockerStatus = lockerPayload.Data.Status;
                            Db.SaveChanges();
                        }
                        break;

                    case CommandTypes.OpenLockersFromAdmin:
                        using (var Db = furchaContext.Create())
                        {
                            object _lockerLock = new object();
                            lock (_lockerLock)
                            {
                                var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<OpenLockerRequest>>>(responseMessage);
                                foreach (var locker in lockersPayload.Data)
                                {
                                    var l = Db.Lockers.FirstOrDefault(x => x.Id == locker.LockerId);
                                    if (l != null)
                                    {
                                        l.LockerStatus = locker.Status;
                                    }
                                }
                                Db.SaveChanges();
                            }
                        }
                        break;

                    case CommandTypes.CreateUserFromAdmin:
                        using (var Db = furchaContext.Create())
                        {
                            object _userLock = new object();
                            lock (_userLock)
                            {
                                var userPayload = JsonSerializer.Deserialize<MqttBaseRequest<MqttUserRequest>>(responseMessage);
                                var dbUser = Db.Users.FirstOrDefault(x => x.Id == userPayload.Data.UserId);
                                dbUser.IsMqtt = userPayload.Data.Success;
                                Db.SaveChanges();
                            }
                        }

                        break;

                    case CommandTypes.CreateBrainModule:
                        using (var Db = furchaContext.Create())
                        {
                            var mqttBrain = JsonSerializer.Deserialize<MqttBaseRequest<MqttCreateBrain>>(responseMessage);
                            var companyId = Db.Companies.FirstOrDefault(x => x.AccountUid == mqttBrain.Data.AccountId).Id;
                            var brains = Db.BrainModules.Where(x => x.CompanyId == companyId).ToList();
                            if (brains.Any(x => x.BrainUid == mqttBrain.Data.BrainUID))
                            {
                                PublishToMqtt<int>(new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.CreateBrainModule
                                }, "webserver/6fa85f64-5717-4562-b3fc-2c963f66afa6/004F00443133510933373933");
                                break; 
                            }

                            Db.BrainModules.Add(new BrainModule
                            {
                                Status = (int)BrainStatuses.New,
                                CompanyId = companyId,
                                IpAddress = mqttBrain.Data.IpAddress,
                                MacAddress = mqttBrain.Data.MacAddress,
                                BrainUid = mqttBrain.Data.BrainUID,
                                Description = mqttBrain.Data.Info,
                                GroupId = null
                            });

                            var added = Db.SaveChanges();
                            if (added > 0)
                            {
                                PublishToMqtt<int>(new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.CreateBrainModule
                                }, "webserver/6fa85f64-5717-4562-b3fc-2c963f66afa6/004F00443133510933373933");
                            }
                        }
                        break;

                    case CommandTypes.AddLockersToBrain:
                        using (var Db = furchaContext.Create())
                        {
                            var lockerCount = JsonSerializer.Deserialize<MqttBaseRequest<MqttLockerCount>>(responseMessage);
                            for (int i = 0; i < lockerCount.Data.LockerCount; i++)
                            {
                                Db.Lockers.Add(new Locker
                                {
                                    LockerType = "unassigned",
                                    PasswordHash = "test"
                                });
                            }

                            var success = Db.SaveChanges();
                            if (success > 0)
                            {
                                PublishToMqtt<int>(new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.AddLockersToBrain
                                }, "webserver/6fa85f64-5717-4562-b3fc-2c963f66afa6/004F00443133510933373933");
                            }
                        }
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
