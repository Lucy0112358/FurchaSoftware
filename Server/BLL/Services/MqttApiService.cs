using FurchaBLL.Interfaces;
using MQTTnet.Client;
using MQTTnet;
using FurchaBLL.MqttModels.Subscribe;
using System.Text.Json;

namespace FurchaBLL.Services
{
    public class MqttApiService : IMqttApiService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _options;

        public bool IsConnected => _mqttClient.IsConnected;

        public MqttApiService(IMqttClient mqttClient, MqttClientOptions options)
        {
            _mqttClient = mqttClient;
            _options = options;
        }

        public async Task ConnectAsync()
        {
            if (!_mqttClient.IsConnected)
                await _mqttClient.ConnectAsync(_options);
        }

        public async Task PublishAsync<T>(MqttBaseRequest<T> command, string companyUID, string brainUID)
        {
            if (!_mqttClient.IsConnected)
                await _mqttClient.ConnectAsync(_options);

            var payload = JsonSerializer.Serialize(command);
            var topic = $"controller/{companyUID}/{brainUID}/commands";
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();

            await _mqttClient.PublishAsync(message);
        }

    }
}
