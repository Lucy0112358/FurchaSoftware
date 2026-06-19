using Microsoft.AspNetCore.SignalR;

namespace FurchaAdminApi.Hubs
{
    public class DoorStatusHub : Hub
    {
        public async Task PublishDoorStatus(int doorId, string status)
        {
            await Clients.All.SendAsync("ReceiveDoorStatus", doorId, status);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

   /*     public async Task JoinDoorGroup(int doorId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"door_{doorId}");
        }

        public async Task LeaveDoorGroup(int doorId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"door_{doorId}");
        }*/
    }
}
