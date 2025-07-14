using FurchaBLL.MqttModels.Subscribe;

namespace FurchaBLL.Interfaces
{
    public interface IMqttApiService
    {
        Task PublishAsync<T>(MqttBaseRequest<T> command, string companyUID, string brainUID);
        bool IsConnected { get; }

        Task ConnectAsync();
    }
}
