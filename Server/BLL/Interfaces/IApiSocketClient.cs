namespace FurchaBLL.Interfaces
{
    public interface IApiSocketClient
    {
        Task SendDoorStatusAsync(int doorId, int status, CancellationToken cancellationToken = default);
    }
}