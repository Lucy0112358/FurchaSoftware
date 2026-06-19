using FurchaBLL.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FurchaBLL.Services
{
    public class ApiSocketClient : IApiSocketClient, IAsyncDisposable
    {
        private readonly ILogger<ApiSocketClient> _logger;
        private readonly HubConnection _connection;

        public ApiSocketClient(IConfiguration configuration, ILogger<ApiSocketClient> logger)
        {
            _logger = logger;

            var hubUrl = configuration["ApiSocket:HubUrl"];
            if (string.IsNullOrWhiteSpace(hubUrl))
            {
                throw new InvalidOperationException("Missing configuration key ApiSocket:HubUrl");
            }

            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _connection.Reconnecting += error =>
            {
                _logger.LogWarning(error, "Reconnecting to API hub.");
                return Task.CompletedTask;
            };

            _connection.Reconnected += connectionId =>
            {
                _logger.LogInformation("Reconnected to API hub. ConnectionId: {ConnectionId}", connectionId);
                return Task.CompletedTask;
            };

            _connection.Closed += error =>
            {
                _logger.LogWarning(error, "Connection to API hub closed.");
                return Task.CompletedTask;
            };
        }

        public async Task SendDoorStatusAsync(int doorId, int status, CancellationToken cancellationToken = default)
        {
            var statusText = status == 2 ? "Closed" : "Open";

            await EnsureConnectedAsync(cancellationToken);

            try
            {
                await _connection.InvokeAsync("PublishDoorStatus", doorId, statusText, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "First attempt to publish door status failed. Retrying once.");
                await ReconnectAsync(cancellationToken);
                await _connection.InvokeAsync("PublishDoorStatus", doorId, statusText, cancellationToken);
            }
        }

        private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                return;
            }

            await ReconnectAsync(cancellationToken);
        }

        private async Task ReconnectAsync(CancellationToken cancellationToken)
        {
            if (_connection.State == HubConnectionState.Connected)
            {
                return;
            }

            if (_connection.State == HubConnectionState.Connecting)
            {
                return;
            }

            await _connection.StartAsync(cancellationToken);
            _logger.LogInformation("Connected to API hub.");
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}