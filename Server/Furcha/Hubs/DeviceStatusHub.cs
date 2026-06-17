using Microsoft.AspNetCore.SignalR;

namespace FurchaAdminApi.Hubs
{
    public class DeviceStatusHub : Hub
    {
        private const string _accountGroupPrefix = "account";

        public static string GetAccountGroup(string accountUID)
        {
            return $"{_accountGroupPrefix}_{accountUID}".ToLowerInvariant();
        }

        public override async Task OnConnectedAsync()
        {
            if (!TryGetAccountUID(out var accountUID))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, GetAccountGroup(accountUID));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        private bool TryGetAccountUID(out string accountUID)
        {
            accountUID = string.Empty;
            var raw = Context.GetHttpContext()?.Request.Query["accountUID"].ToString();

            if (!string.IsNullOrEmpty(raw))
            {
                accountUID = raw;
                return true;
            }

            return false;
        }
    }
}
