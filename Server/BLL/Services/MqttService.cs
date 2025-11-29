using FurchaBLL.Constants;
using FurchaBLL.MqttModels.Subscribe;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using System.Text.Json;
using System.Text;
using FurchaDAL.Models;
using FurchaBLL.MqttModels.Publish;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Server;
using FurchaBLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

public class MqttService
{
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttOptions;
    private readonly ILogger<MqttService> _logger;
    // private readonly furchaContext Db;
    private readonly IServiceScopeFactory _scopeFactory;
    //   private readonly IDoorStateService _doorStateService;

    public MqttService(IMqttClient mqttClient, MqttClientOptions mqttOptions, ILogger<MqttService> logger, /*IDoorStateService doorStateService,*/ /*furchaContext db,*/ IServiceScopeFactory scopeFactory)
    {
        _mqttClient = mqttClient;
        _mqttOptions = mqttOptions;
        _logger = logger;
        // Db = db;
        _scopeFactory = scopeFactory;
        // _doorStateService = doorStateService;
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
                    case CommandTypes.UpdateDoorStatus:
                        using (var Db = furchaContext.Create())
                        {
                            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<OpenLockerRequest>>(responseMessage);
                            var brainUid = topic.Split("/")[2];
                            var brain = Db.BrainModules.First(b => b.BrainUid == brainUid);

                            var dbLocker = Db.Lockers.Where(x => x.ExternalId == lockerPayload.Data.Number && x.BrainId == brain.Id).FirstOrDefault();
                            dbLocker.LockerStatus = lockerPayload.Data.Status;
                            var res = Db.SaveChanges();
                            if (res > 0)
                            {
                                using var httpClient = new HttpClient();
                                string baseUrl = "http://192.168.0.129:1010"; // or http://127.0.0.1:1010
                                string endpoint = $"/api/locker/test-door-status?doorId={dbLocker.Id}&status={(lockerPayload.Data.Status == 2 ? "Closed" : "Open")}";

                                var response = await httpClient.GetAsync(baseUrl + endpoint);

                                if (!response.IsSuccessStatusCode)
                                {
                                    _logger.LogWarning($"Failed to notify door status. Response: {response.StatusCode}");
                                }

                            }

                        }
                        break;

                    case CommandTypes.OpenLockersFromAdmin:
                        using (var Db = furchaContext.Create())
                        {
                            object _lockerLock = new object();
                            lock (_lockerLock)
                            {
                                var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<OpenLockerRequest>>>(responseMessage);
                                var brainUid = topic.Split("/")[2];
                                var brain = Db.BrainModules.First(b => b.BrainUid == brainUid);

                                foreach (var locker in lockersPayload.Data)
                                {
                                    var l = Db.Lockers.Where(x => x.ExternalId == locker.Number && x.BrainId == brain.Id).FirstOrDefault();
                                    if (l != null)
                                    {
                                        l.LockerStatus = locker.Status;
                                        using var httpClient = new HttpClient();
                                        string baseUrl = "http://localhost:1010"; // or http://127.0.0.1:1010
                                        string endpoint = $"/api/locker/test-door-status?doorId={l.Id}&status={(locker.Status == 2 ? "Closed" : "Open")}";

                                        var response = httpClient.GetAsync(baseUrl + endpoint);
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

                            if (brains.Any(x => x.BrainUid == mqttBrain.Data.BrainUid))
                            {
                                PublishToMqtt<int>(new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.CreateBrainModule
                                }, topic.Replace("webserver", "controller"));
                                break;
                            }

                            Db.BrainModules.Add(new FurchaDAL.Models.BrainModule
                            {
                                Status = (int)BrainStatuses.New,
                                CompanyId = companyId,
                                IpAddress = mqttBrain.Data.IpAddress,
                                MacAddress = mqttBrain.Data.MacAddress,
                                BrainUid = mqttBrain.Data.BrainUid,
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
                                }, topic.Replace("webserver", "controller"));
                            }
                        }
                        break;

                    case CommandTypes.AddLockersToBrain:
                        using (var Db = furchaContext.Create())
                        {
                            var mqttRequest = JsonSerializer.Deserialize<MqttBaseRequest<MqttLockerCount>>(responseMessage);

                            var brain = Db.BrainModules.Include(b => b.Lockers).FirstOrDefault(x => x.BrainUid == mqttRequest.Data.BrainUid);
                            var count = brain.Lockers.Count;
                            if (count < mqttRequest.Data.LockerCount)
                            {
                                for (int i = 0; i < (mqttRequest.Data.LockerCount - count); i++)
                                {
                                    Db.Lockers.Add(new FurchaDAL.Models.Locker
                                    {
                                        LockerType = brain.Lockers.FirstOrDefault()?.LockerType ?? 1,
                                        PasswordHash = "test",
                                        BrainId = brain.Id,
                                        ExternalId = (brain.Lockers.FirstOrDefault()?.ExternalId + 1) ?? (i + 1)
                                    });
                                }
                            }
                            if (count > mqttRequest.Data.LockerCount)
                            {
                                for (int i = 0; i < (count - mqttRequest.Data.LockerCount); i++)
                                {
                                    var l = brain.Lockers.OrderByDescending(x => x.ExternalId).ToList();
                                    Db.Lockers.Remove(l[i]);
                                }
                            }
                            var success = Db.SaveChanges();
                            if (success > 0)
                            {
                                PublishToMqtt<int>(new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.AddLockersToBrain
                                }, topic.Replace("webserver", "controller"));
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

        var result = await _mqttClient.PublishAsync(message, CancellationToken.None);
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
                .WithRetainFlag(false)
                .Build();

            await _mqttClient.PublishAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish MQTT message to topic {Topic}", topic);
        }
    }
}
