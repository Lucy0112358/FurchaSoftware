namespace FurchaBLL.Interfaces
{
    public interface IDeviceStatusNotifier
    {
        Task NotifyBrainStatusAsync(int companyId, int brainId, string status);
    }
}
