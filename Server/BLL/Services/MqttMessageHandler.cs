using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Publish;
using FurchaBLL.MqttModels.Subscribe;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FurchaBLL.Services
{
    /// <summary>
    /// Handles incoming MQTT messages and processes them according to command types.
    /// </summary>
    public class MqttMessageHandler : IMqttMessageHandler
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMqttService _mqttService;
        private readonly IApiSocketClient _apiSocketClient;
        private readonly ILogger<MqttMessageHandler> _logger;

        public MqttMessageHandler(
            IServiceScopeFactory scopeFactory,
            IMqttService mqttService,
            IApiSocketClient apiSocketClient,
            ILogger<MqttMessageHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _mqttService = mqttService;
            _apiSocketClient = apiSocketClient;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task HandleAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            if (eventArgs?.ApplicationMessage == null)
            {
                _logger.LogWarning("Received null MQTT message event.");
                return;
            }

            var topic = eventArgs.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);

            _logger.LogInformation("Received MQTT message on topic {Topic}", topic);

            try
            {
                if (!topic.StartsWith("webserver/"))
                {
                    _logger.LogDebug("Message from topic {Topic} is not from webserver. Ignoring.", topic);
                    return;
                }

                // webserver/<accountUID>/<brainUid>/status
                if (topic.EndsWith("/status", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleBrainStatusAsync(topic, payload);
                    return;
                }

                var message = JsonSerializer.Deserialize<MqttBaseRequest<object>>(payload);
                if (message == null)
                {
                    _logger.LogWarning("Failed to deserialize message from topic {Topic}", topic);
                    return;
                }

                var commandType = (CommandTypes)message.Command;

                switch (commandType)
                {
                    case CommandTypes.UpdateDoorStatus:
                        await HandleUpdateDoorStatusAsync(topic, payload);
                        break;

                    case CommandTypes.OpenLockersFromAdmin:
                        await HandleOpenLockersFromAdminAsync(topic, payload);
                        break;

                    case CommandTypes.CreateUserFromAdmin:
                        await HandleCreateUserFromAdminAsync(topic, payload);
                        break;

                    case CommandTypes.CreateBrainModule:
                        await HandleCreateBrainModuleAsync(topic, payload);
                        break;

                    case CommandTypes.AddLockersToBrain:
                        await HandleAddLockersToBrainAsync(topic, payload);
                        break;

                    case CommandTypes.ChangeType:
                        await HandleChangeTypeAsync(topic, payload);
                        break;

                    case CommandTypes.SuspendLocker:
                        await HandleSuspendLockerDoorAsync(topic, payload);
                        break;
                    case CommandTypes.SendLockerMode:
                        await HandleLockerModeChangeAsync(topic, payload);
                        break;
                    default:
                        _logger.LogWarning("Unknown command type {CommandType} from topic {Topic}", commandType, topic);
                        break;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogInformation("Raw MQTT payload (topic: {Topic}): {Payload}", topic, payload);
                _logger.LogError(ex, "Failed to deserialize MQTT payload from topic {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling MQTT message from topic {Topic}", topic);
            }
        }

        private async Task HandleChangeTypeAsync(string topic, string payload)
        {
            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<IEnumerable<ChangeTypeRequest>>>(payload);
            if (lockerPayload?.Data == null || !lockerPayload.Data.Any())
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>())
                {
                    var brainUid = ExtractBrainUidFromTopic(topic);
                    if (string.IsNullOrEmpty(brainUid))
                    {
                        _logger.LogWarning("Could not extract brain UID from topic {Topic}", topic);
                        return;
                    }

                    var brain = await dbContext.BrainModules.FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                    if (brain == null)
                    {
                        _logger.LogWarning("Brain module not found for UID {BrainUid}", brainUid);
                        return;
                    }

                    var allLockerTypes = await dbContext.LockerTypes
                            .Where(lt => 1 == 1)
                            .ToListAsync();

                    foreach (var change in lockerPayload.Data)
                    {
                        // Each item specifies a device type — either "Locker" or "Door".
                        // Locker and Door statuses are handled separately based on change.Type.
                        if ("Locker".Equals(change.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            var dbLocker = await dbContext.Lockers
                                .FirstOrDefaultAsync(x => x.ExternalId == change.Number && x.BrainId == brain.Id);
                            if (dbLocker == null)
                            {
                                _logger.LogWarning("Locker not found for brain {BrainId} and external ID {ExternalId}",
                                    brain.Id, change.Number);
                                return;
                            }

                            var lockerType = change.ChangeType switch
                            {
                                0 => allLockerTypes.FirstOrDefault(lt => lt.Type.ToLower() == "personal"),
                                1 => allLockerTypes.FirstOrDefault(lt => lt.Type.ToLower() == "common"),
                                2 => allLockerTypes.FirstOrDefault(lt => lt.Type.ToLower() == "parcel"),
                                3 => allLockerTypes.FirstOrDefault(lt => lt.Type.ToLower() == "unspecified"),
                                _ => null
                            };

                            if (lockerType == null)
                            {
                                _logger.LogWarning("Locker type not found for change type {ChangeType}", change.ChangeType);
                                return;
                            }

                            dbLocker.LockerType = lockerType.Id;

                            var result = await dbContext.SaveChangesAsync();

                            await _mqttService.PublishAsync<int>(
                                new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.ChangeType
                                },
                                topic.Replace("webserver", "controller")
                            );
                        }
                        else if ("Door".Equals(change.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            // TODO: Implement Door type update logic.
                        }
                        else
                        {
                            _logger.LogWarning("Unknown device type '{Type}' received.", change.Type);
                        }
                    }
                }
            }
        }

        private async Task HandleLockerModeChangeAsync(string topic, string payload)
        {
            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<IEnumerable<LockerModeChangeRequest>>>(payload);
            if (lockerPayload?.Data == null || !lockerPayload.Data.Any())
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>())
                {
                    var brainUid = ExtractBrainUidFromTopic(topic);
                    if (string.IsNullOrEmpty(brainUid))
                    {
                        _logger.LogWarning("Could not extract brain UID from topic {Topic}", topic);
                        return;
                    }

                    var brain = await dbContext.BrainModules.FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                    if (brain == null)
                    {
                        _logger.LogWarning("Brain module not found for UID {BrainUid}", brainUid);
                        return;
                    }

                    foreach (var locker in lockerPayload.Data)
                    {
                        // Each item specifies a device type — either "Locker" or "Door".
                        // Suspension for lockers and doors is handled separately based on locker.Type.
                        if ("Locker".Equals(locker.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            var dbLocker = await dbContext.Lockers
                                .FirstOrDefaultAsync(x => x.ExternalId == locker.Number && x.BrainId == brain.Id);
                            if (dbLocker == null)
                            {
                                _logger.LogWarning("Locker not found for brain {BrainId} and external ID {ExternalId}",
                                    brain.Id, locker.Number);
                                return;
                            }

                            dbLocker.LockerStatus = locker.Mode switch
                            {
                                LockerMode.Free => 1,
                                LockerMode.Occupied => 2,
                                _ => 1
                            };

                            var result = await dbContext.SaveChangesAsync();

                            await _mqttService.PublishAsync<int>(
                                new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.SendLockerMode
                                },
                                topic.Replace("webserver", "controller")
                            );
                        }
                        else if ("Door".Equals(locker.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            // TODO: Implement Door type update logic.
                        }
                        else
                        {
                            _logger.LogWarning("Unknown device type '{Type}' received.", locker.Type);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обрабатывает online/offline брейна, полученный по MQTT Last Will / birth.
        /// Топик: webserver/&lt;accountUID&gt;/&lt;brainUid&gt;/status (suffix /status уже
        /// проверен в HandleAsync). brainUid — parts[2], как и у обычных команд.
        /// Payload: {"Status":"Online"} (retained birth после connect) либо
        /// {"Status":"Offline"} (LWT от брокера при обрыве). Обновляет
        /// BrainModule.IsOnline.
        /// </summary>
        private async Task HandleBrainStatusAsync(string topic, string payload)
        {
            var brainUid = ExtractBrainUidFromTopic(topic);
            if (string.IsNullOrEmpty(brainUid))
            {
                _logger.LogWarning("Could not extract brain UID from status topic {Topic}", topic);
                return;
            }

            bool isOnline;
            try
            {
                var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(payload);

                if (dictionary == null || (!dictionary.TryGetValue("Status", out var status)
                       && !dictionary.TryGetValue("status", out status)))
                {
                    _logger.LogWarning("Bad brain status payload on {Topic}: {Payload}", topic, payload);
                    return;
                }

                isOnline = "Online".Equals(status, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Bad brain status payload on {Topic}: {Payload}", topic, payload);
                return;
            }

            using (var scope = _scopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>())
                {
                    var brain = await dbContext.BrainModules.Include(b => b.Company).FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                    if (brain == null)
                    {
                        _logger.LogWarning("Brain module not found for UID {BrainUid} (status update)", brainUid);
                        return;
                    }

                    if (brain.IsOnline == isOnline)
                        return;

                    brain.IsOnline = isOnline;
                    await dbContext.SaveChangesAsync();

                    await NotifyBrainServiceAsync(brain.Company.AccountUid.ToString(), brain.Id, isOnline ? "Online" : "Offline");

                    _logger.LogInformation("Brain {BrainUid} is now {State}", brainUid, isOnline ? "Online" : "Offline");
                }
            }
        }

        private async Task HandleSuspendLockerDoorAsync(string topic, string payload)
        {
            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<IEnumerable<SuspendLockerRequest>>>(payload);
            if (lockerPayload?.Data == null || !lockerPayload.Data.Any())
                return;

            using (var scope = _scopeFactory.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>())
                {
                    var brainUid = ExtractBrainUidFromTopic(topic);
                    if (string.IsNullOrEmpty(brainUid))
                    {
                        _logger.LogWarning("Could not extract brain UID from topic {Topic}", topic);
                        return;
                    }

                    var brain = await dbContext.BrainModules.FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                    if (brain == null)
                    {
                        _logger.LogWarning("Brain module not found for UID {BrainUid}", brainUid);
                        return;
                    }

                    foreach (var locker in lockerPayload.Data)
                    {
                        // Each item specifies a device type — either "Locker" or "Door".
                        // Suspension for lockers and doors is handled separately based on locker.Type.
                        if ("Locker".Equals(locker.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            var dbLocker = await dbContext.Lockers
                                .FirstOrDefaultAsync(x => x.ExternalId == locker.Number && x.BrainId == brain.Id);
                            if (dbLocker == null)
                            {
                                _logger.LogWarning("Locker not found for brain {BrainId} and external ID {ExternalId}",
                                    brain.Id, locker.Number);
                                return;
                            }

                            dbLocker.IsActive = locker.Status;

                            var result = await dbContext.SaveChangesAsync();

                            await _mqttService.PublishAsync<int>(
                                new MqttBaseRequest<int>
                                {
                                    Operation = (int)OperationTypes.Success,
                                    Command = (int)CommandTypes.SuspendLocker
                                },
                                topic.Replace("webserver", "controller")
                            );
                        }
                        else if ("Door".Equals(locker.Type, StringComparison.OrdinalIgnoreCase))
                        {
                            // TODO: Implement Door type update logic.
                        }
                        else
                        {
                            _logger.LogWarning("Unknown device type '{Type}' received.", locker.Type);
                        }
                    }
                }
            }
        }

        private async Task HandleUpdateDoorStatusAsync(string topic, string payload)
        {
            var lockerPayload = JsonSerializer.Deserialize<MqttBaseRequest<IEnumerable<OpenLockerRequest>>>(payload);
            if (lockerPayload?.Data == null || !lockerPayload.Data.Any())
                return;

            using var scope = _scopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>();
            try
            {
                var brainUid = ExtractBrainUidFromTopic(topic);
                if (string.IsNullOrEmpty(brainUid))
                {
                    _logger.LogWarning("Could not extract brain UID from topic {Topic}", topic);
                    return;
                }

                var brain = await dbContext.BrainModules.FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                if (brain == null)
                {
                    _logger.LogWarning("Brain module not found for UID {BrainUid}", brainUid);
                    return;
                }
                foreach (var lp in lockerPayload.Data)
                {
                    // Each item specifies a device type — either "Locker" or "Door".
                    // Locker and Door statuses are handled separately based on lp.Type.
                    if ("Locker".Equals(lp.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        var dbLocker = await dbContext.Lockers
                            .FirstOrDefaultAsync(x => x.ExternalId == lp.Number && x.BrainId == brain.Id);

                        if (dbLocker == null)
                        {
                            _logger.LogWarning("Locker not found for brain {BrainId} and external ID {ExternalId}",
                                brain.Id, lp.Number);
                            return;
                        }

                        dbLocker.LockerStatus = lp.Status;

                        var result = await dbContext.SaveChangesAsync();

                        if (result > 0)
                        {
                            await NotifyDoorServiceAsync(dbLocker.Id, lp.Status);
                        }
                    }
                    else if ("Door".Equals(lp.Type, StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO: Implement Door status update logic.
                    }
                    else
                    {
                        _logger.LogWarning("Unknown device type '{Type}' received.", lp.Type);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling UpdateDoorStatus command");
            }

        }

        private async Task HandleOpenLockersFromAdminAsync(string topic, string payload)
        {
            var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<OpenLockerRequest>>>(payload);
            if (lockersPayload?.Data == null || lockersPayload.Data.Count == 0)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>();

            try
            {
                var brainUid = ExtractBrainUidFromTopic(topic);
                if (string.IsNullOrEmpty(brainUid))
                {
                    _logger.LogWarning("Could not extract brain UID from topic {Topic}", topic);
                    return;
                }

                var brain = await dbContext.BrainModules.FirstOrDefaultAsync(b => b.BrainUid == brainUid);
                if (brain == null)
                {
                    _logger.LogWarning("Brain module not found for UID {BrainUid}", brainUid);
                    return;
                }

                foreach (var locker in lockersPayload.Data)
                {
                    var dbLocker = await dbContext.Lockers
                        .FirstOrDefaultAsync(x => x.ExternalId == locker.Number && x.BrainId == brain.Id);

                    if (dbLocker != null)
                    {
                        dbLocker.LockerStatus = locker.Status;
                        await NotifyDoorServiceAsync(dbLocker.Id, locker.Status);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling OpenLockersFromAdmin command");
            }
        }

        private async Task HandleCreateUserFromAdminAsync(string topic, string payload)
        {
            var userPayload = JsonSerializer.Deserialize<MqttBaseRequest<MqttUserRequest>>(payload);
            if (userPayload?.Data == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>();

            try
            {
                var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userPayload.Data.UserId);
                if (dbUser != null)
                {
                    dbUser.IsMqtt = userPayload.Data.Success;
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    _logger.LogWarning("User not found with ID {UserId}", userPayload.Data.UserId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling CreateUserFromAdmin command");
            }
        }

        private async Task HandleCreateBrainModuleAsync(string topic, string payload)
        {
            var mqttBrain = JsonSerializer.Deserialize<MqttBaseRequest<MqttCreateBrain>>(payload);
            if (mqttBrain?.Data == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>();

            try
            {
                var company = await dbContext.Companies
                    .FirstOrDefaultAsync(x => x.AccountUid == mqttBrain.Data.AccountId);

                if (company == null)
                {
                    _logger.LogWarning("Company not found for account ID {AccountId}", mqttBrain.Data.AccountId);
                    return;
                }

                var brain = await dbContext.BrainModules
                    .FirstOrDefaultAsync(x => x.CompanyId == company.Id && x.BrainUid == mqttBrain.Data.BrainUid);

                if (brain != null)
                {
                    brain.IpAddress = mqttBrain.Data.IpAddress;
                    brain.MacAddress = mqttBrain.Data.MacAddress;
                    brain.Description = mqttBrain.Data.Info;
                    brain.Status = brain.Status == 2 ? brain.Status : (int)BrainStatuses.New;
                }
                else
                {
                    brain = new BrainModule
                    {
                        Status = (int)BrainStatuses.New,
                        CompanyId = company.Id,
                        IpAddress = mqttBrain.Data.IpAddress,
                        MacAddress = mqttBrain.Data.MacAddress,
                        BrainUid = mqttBrain.Data.BrainUid,
                        Description = mqttBrain.Data.Info,
                        GroupId = null
                    };

                    dbContext.BrainModules.Add(brain);
                }

                var saved = await dbContext.SaveChangesAsync();

                await _mqttService.PublishAsync<int>(
                        new MqttBaseRequest<int>
                        {
                            Operation = (int)OperationTypes.Success,
                            Command = (int)CommandTypes.CreateBrainModule
                        },
                        topic.Replace("webserver", "controller"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling CreateBrainModule command");
            }
        }

        private async Task HandleAddLockersToBrainAsync(string topic, string payload)
        {
            var mqttRequest = JsonSerializer.Deserialize<MqttBaseRequest<MqttLAddLockersInput>>(payload);
            if (mqttRequest?.Data == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>();

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var brain = await dbContext.BrainModules
                        .Include(b => b.Lockers)
                        .FirstOrDefaultAsync(x => x.BrainUid == mqttRequest.Data.BrainUid);

                    if (brain == null)
                    {
                        _logger.LogWarning("Brain module not found for UID {BrainUid}", mqttRequest.Data.BrainUid);
                        return;
                    }

                    var mqttLockerIds = mqttRequest.Data.Lockers
                        ?.Select(x => x.ExternalIds)
                        ?.ToList() ?? [];

                    List<int> dbLockerExternalIds = brain.Lockers
                        ?.Where(x => x.ExternalId.HasValue)
                        ?.Select(x => x.ExternalId.GetValueOrDefault())
                        ?.ToList() ?? [];

                    List<int> lockersToAdd = mqttLockerIds
                        .Except(dbLockerExternalIds)
                        .ToList();

                    var lockersToAddEntities = await dbContext.Lockers
                        .Where(x => x.ExternalId != null &&
                            lockersToAdd.Contains(x.ExternalId.Value) &&
                            x.BrainId == brain.Id)
                        .ToListAsync();

                    // Find ExternalIds that don't exist in DB
                    var externalIdsNotInDb = lockersToAdd
                        .Except(lockersToAddEntities.Select(x => x.ExternalId.Value))
                        .ToList();

                    // Create new Locker instances for IDs not in DB
                    if (externalIdsNotInDb.Any())
                    {
                        var commonType = await dbContext.LockerTypes
                            .FirstOrDefaultAsync(lt => lt.Type.ToLower() == "common");

                        if (commonType == null)
                        {
                            _logger.LogError("Common locker type not found in database");
                            throw new InvalidOperationException("Common locker type not found in database");
                        }

                        // Calculate the next available locker number for this brain
                        var maxExistingNumber = brain.Lockers?.Any() == true
                            ? brain.Lockers.Where(l => l.Number.HasValue).Max(l => (int?)l.Number.Value) ?? 0 : 0;

                        var newLockers = externalIdsNotInDb
                            .OrderBy(id => id)
                            .Select((externalId, index) =>
                            {
                                var mqttLocker = mqttRequest?.Data?.Lockers?.FirstOrDefault(l => l.ExternalIds == externalId);

                                return new Locker
                                {
                                    ExternalId = externalId,
                                    BrainId = brain.Id,
                                    IsActive = 1, // Active locker
                                    IsDeleted = false, // Not deleted
                                    IsOpen = 0, // Starts closed
                                    LockerStatus = 1, // Default status (free/available)
                                    LockerType = commonType.Id, // Common type for newly added lockers
                                    Number = (decimal)(maxExistingNumber + index + 1), // Sequential numbering
                                    PasswordHash = null, // Set when user assigns password
                                    ReaderGroupId = mqttLocker?.ReaderGroupId // Set ReaderGroupId from mqttLocker if available
                                };
                            })
                            .ToList();

                        await dbContext.Lockers.AddRangeAsync(newLockers);
                        lockersToAddEntities.AddRange(newLockers);
                    }

                    foreach (var locker in lockersToAddEntities)
                    {
                        locker.BrainId = brain.Id;
                    }

                    var success = await dbContext.SaveChangesAsync();

                    if (success > 0)
                    {
                        await _mqttService.PublishAsync(
                            new MqttBaseRequest<MqttLAddLockersInput>
                            {
                                Operation = (int)OperationTypes.Success,
                                Command = (int)CommandTypes.AddLockersToBrain,
                                Data = new MqttLAddLockersInput
                                {
                                    BrainUid = mqttRequest.Data.BrainUid,
                                    ChunkIndex = mqttRequest.Data.ChunkIndex,
                                    TotalChunks = lockersToAddEntities.Count,
                                    Lockers = lockersToAddEntities.Select(l => new BrainLockers
                                    {
                                        ExternalIds = l.ExternalId.GetValueOrDefault(),
                                        ReaderGroupId = l.ReaderGroupId.GetValueOrDefault(),
                                    }).ToList(),
                                }
                            },
                            topic.Replace("webserver", "controller"));
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(); // undo partial writes
                    _logger.LogError(ex, "Error handling AddLockersToBrain command");
                }
            }
        }

        private async Task NotifyDoorServiceAsync(int doorId, int status)
        {
            try
            {
                await _apiSocketClient.SendDoorStatusAsync(doorId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying door service for door ID {DoorId}", doorId);
            }
        }

        private async Task NotifyBrainServiceAsync(string accountUID, int brainId, string status)
        {
            try
            {
                using var httpClient = new HttpClient();
                var endpoint = $"/api/Modules/send-brain-status?accountUID={accountUID}&brainId={brainId}&status={status}";
                var response = await httpClient.GetAsync(_doorServiceBaseUrl + endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to notify brain service. Account UID: {AccountUID}, Brain ID: {BrainId}, Status: {Status}, Response: {StatusCode}",
                        accountUID, brainId, status, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying brain service for account UID {AccountUID}", accountUID);
            }
        }

        private static string ExtractBrainUidFromTopic(string topic)
        {
            var parts = topic.Split("/");
            return parts.ElementAtOrDefault(2);
        }
    }
}
