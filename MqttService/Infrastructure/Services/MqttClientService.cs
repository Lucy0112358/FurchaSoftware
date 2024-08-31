using Domain.Enums;
using MQTTnet;
using MQTTnet.Client;
using MqttService.Application.Interfaces;
using MqttService.Application.Models.MqttRequest;
using MqttService.Application.Repositories;
using MqttService.Application.Services;
using System.Text;
using System.Text.Json;

namespace MqttService.Infrastructure.Services
{
    public class MqttClientService : IMqttClientService
    {
        private IMqttClient mqttClient;
        private readonly CardLockerRepository cardLockerRepository;
        private CardLockerService cardLockerService;

        public MqttClientService(CardLockerRepository cardLockerRepository)
        {
            cardLockerService = new CardLockerService(cardLockerRepository);
            mqttClient = InitializeClient().GetAwaiter().GetResult();
        }

        public void HandleRequest(string topic, string message)
        {
            MqttRequest mqttRequest=JsonSerializer.Deserialize<MqttRequest>(message);

            string response = cardLockerService.OpenLocker(mqttRequest)!=MqttErrorCodeEnum.Success ? "ACCESS_DENIED":"ACCESS_GRANTED";

            SendResponse(Guid.NewGuid(), mqttRequest.lockerId, response);
        }

        public void SendResponse(Guid guid, int? lockerId, string response)
        {
            var responseTopic = $"{guid}/{lockerId}/response";
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(responseTopic)
                .WithPayload(Encoding.UTF8.GetBytes(response))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            mqttClient.PublishAsync(mqttMessage).Wait();
            Console.WriteLine($"Response sent: {response} to topic {responseTopic}");
        }

        private async Task<IMqttClient> InitializeClient()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Server")
                .WithTcpServer("broker.hivemq.com", 1883)
                //.WithCredentials("Andresuga", "Andresuga0713.")
                //.WithTls()
                .Build();



            try
            {
                var connectResult = await client.ConnectAsync(options).ConfigureAwait(false);
                Console.WriteLine($"{options.ClientId}: Connecting to {options.ClientId} on port {options.Credentials} ...");
                if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine($"Connect failed: {connectResult.ReasonString}");
                    Environment.Exit(-1);
                }
                client.ApplicationMessageReceivedAsync += e =>
                {
                    var topic = e.ApplicationMessage.Topic;
                    var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    Console.WriteLine($"MessageReceived: {message} on topic {topic}");
                    HandleRequest(topic, message);
                    return Task.CompletedTask;
                };


                Console.WriteLine("Server connected to MQTT broker.");
                client.ConnectedAsync += async e =>
                {
                    Console.WriteLine("Server connected successfully with MQTT Broker.");
                    await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("lockers/+/request").Build());
                    Console.WriteLine("Subscribed to topic lockers/+/request");
                };

                return client;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to the MQTT Broker: {ex.Message}");
                Environment.Exit(-1);
                return null;
            }
        }
    }
}
