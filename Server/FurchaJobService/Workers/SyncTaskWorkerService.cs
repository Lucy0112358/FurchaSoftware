using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;
using FurchaBLL.Services;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;
using MQTTnet.Protocol;
using System.Text.Json;

namespace FurchaJobService.Workers
{
    public class SyncTaskWorkerService : BackgroundService
    {
        private readonly ILogger<SyncTaskWorkerService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMqttService _mqttService;

        private const int MinPollMs = 100;
        private const int MaxPollMs = 200;

        public SyncTaskWorkerService(
            ILogger<SyncTaskWorkerService> logger,
            IServiceScopeFactory scopeFactory,
            IMqttService mqttService)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _mqttService = mqttService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SyncTaskWorkerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessCycleAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    // Never let one bad cycle kill the loop.
                    _logger.LogError(ex, "SyncTaskWorkerService cycle failed.");
                }

                try
                {
                    await Task.Delay(Random.Shared.Next(MinPollMs, MaxPollMs + 1), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break; // normal shutdown
                }
            }

            _logger.LogInformation("SyncTaskWorkerService stopping.");
        }

        private async Task ProcessCycleAsync(CancellationToken stoppingToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {

                var syncTaskService = scope.ServiceProvider.GetRequiredService<SyncTaskService>();
                var syncTask = await syncTaskService.ClaimNextAsync();

                if (syncTask == null)
                {
                    return;
                }

                using (var dbContext = scope.ServiceProvider.GetRequiredService<furchaContext>())
                {
                    var brain = await dbContext.BrainModules
                        .FirstOrDefaultAsync(b => b.BrainUid == syncTask.BrainUid);

                    if (brain == null || !brain.IsOnline)
                    {
                        return;
                    }

                    var request = new MqttBaseRequest<JsonElement>
                    {
                        Command = syncTask.CommandType,
                        Operation = syncTask.Operation,
                        TaskId = syncTask.Id,
                        Data = JsonSerializer.Deserialize<JsonElement>(syncTask.Payload),
                        ReceivedDate = DateTime.UtcNow
                    };

                    var topic = $"controller/{syncTask.AccountUid}/{syncTask.BrainUid}";

                    var result = await _mqttService.PublishAsync(request, topic,
                        withRetainFlag: false,
                        qos: MqttQualityOfServiceLevel.AtLeastOnce);

                    if (result == null || !result.IsSuccess)
                    {
                        _logger.LogWarning($"Publish failed for SyncTask " +
                            $"{syncTask.Id} to {topic}; will retry after the lease expires.");

                        await syncTaskService.MarkFailedAsync(syncTask, "Failed to publish MQTT message.");
                        return;
                    }

                    _logger.LogInformation("Published SyncTask {TaskId} to {Topic}.", syncTask.Id, topic);

                    await syncTaskService.MarkSentAsync(syncTask);
                }
            }
        }
    }
}
