using FurchaAdminApi.Hubs;
using FurchaBLL.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FurchaAdminApi.Services
{
    public class SignalRNotificationService : IDoorStateService
    {
        private readonly IHubContext<DoorStatusHub> _hubContext;

        public SignalRNotificationService(IHubContext<DoorStatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyDoorStatusAsync(int doorId, string status)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveDoorStatus", doorId, status);
        }
    }
}
