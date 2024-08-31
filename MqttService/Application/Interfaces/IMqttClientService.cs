namespace MqttService.Application.Interfaces
{
    public interface IMqttClientService
    {
        void HandleRequest(string topic, string message);
        void SendResponse(Guid guid, int? lockerId, string response);
    }
}
