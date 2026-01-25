using FurchaBLL.MqttModels.Subscribe;

namespace FurchaBLL.Interfaces
{
    public interface IMqttApiService
    {
        Task PublishMqttCommands<T>(MqttBaseRequest<T> command, string companyUID, string brainUID);
        bool IsConnected { get; }

        Task ConnectToMqtt();
    }
}
