using FurchaAdminApi.Hubs;
using FurchaBLL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FurchaAdminApi.Services
{
    public class DeviceStatusNotificationService : IDeviceStatusNotifier
    {
        private readonly IHubContext<DeviceStatusHub> _hubContext;

        public DeviceStatusNotificationService(IHubContext<DeviceStatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyBrainStatusAsync(string accountUID, int brainId, string status)
        {
            return _hubContext.Clients
                .Group(DeviceStatusHub.GetAccountGroup(accountUID))
                .SendAsync("ReceiveBrainStatus", brainId, status);
        }
    }
}
