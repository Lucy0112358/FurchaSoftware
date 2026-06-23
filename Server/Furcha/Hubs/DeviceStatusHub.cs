using Microsoft.AspNetCore.SignalR;

namespace FurchaAdminApi.Hubs
{
    public class DeviceStatusHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (!TryGetCompanyId(out var companyId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, HubGroupNames.GetCompanyGroup(companyId));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private bool TryGetCompanyId(out int companyId)
        {
            companyId = 0;
            var raw = Context.GetHttpContext()?.Request.Query["companyId"].ToString();

            return !string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out companyId);
        }
    }
}
