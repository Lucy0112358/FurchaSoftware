namespace MqttService.Application.Interfaces
{
    public interface IMqttClientService
    {
        void HandleRequest(string topic, string message);
        void SendResponseSet(int? branchUID, int brainUID, string response);
        void SendResponse(int? branchUID, int? lockerId, string response);
    }
}
