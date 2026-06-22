namespace FurchaBLL.Interfaces
{
    public interface IDeviceStatusNotifier
    {
        Task NotifyBrainStatusAsync(string accountUID, int brainId, string status);
    }
}
