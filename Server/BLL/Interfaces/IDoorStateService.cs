namespace FurchaBLL.Interfaces
{
    public interface IDoorStateService
    {
        Task NotifyDoorStatusAsync(int doorId, string status);
    }
}
