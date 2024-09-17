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
            var branchUIDStr = topic.Split('/')[1];
            if (!int.TryParse(branchUIDStr, out int branchUID))
            {
                //handle the case when converted wasn't successfully
            }
            if (topic.StartsWith("get/"))
            {
                CardModel mqttRequestGet = JsonSerializer.Deserialize<CardModel>(message);
                if (mqttRequestGet != null)
                {
                    //handle the null request case
                }
                if (!mqttRequestGet.CardNumber.HasValue)
                {
                    // handle the null CardNumber case
                }
                var response = cardLockerService.GetLockersByCardId(mqttRequestGet.CardNumber.Value);
                var lockerIds = response.Select(x => x.Id).ToList();
                var brainUIDStr = topic.Split('/')[2];
                if (!int.TryParse(brainUIDStr, out int brainUID))
                {
                    // brainUID error
                }
                SendResponseSet(branchUID, brainUID, string.Join(";", lockerIds));
            }
            else if (topic.StartsWith("connection/"))
            {
                // Processing the topic connection/BranchUID
                Console.WriteLine($"Received connection message: {message} on topic {topic}");
                BrainModuleModel mqttRequestConnection = JsonSerializer.Deserialize<BrainModuleModel>(message);
                Console.WriteLine($"Brain: {mqttRequestConnection.Brain} IP Adress: {mqttRequestConnection.IP}");
                SubscribeToBrainTopic(branchUID, mqttRequestConnection.Brain);
            }
/*            else
            {
                MqttRequest mqttRequest = JsonSerializer.Deserialize<MqttRequest>(message);
                string responseOne = cardLockerService.OpenLocker(mqttRequest) != MqttErrorCodeEnum.Success ? "ACCESS_DENIED" : "ACCESS_GRANTED";
                SendResponse(branchUID, mqttRequest.lockerId, responseOne);
            }*/
        }

        public void SendResponseSet(int? branchUID, int brainUID, string response)
        {
            // set/BranchUID/BrainUID/cards/{number of the locker that can be opened}
            var responseTopic = $"set/{branchUID}/{brainUID}/cards";
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(responseTopic)
                .WithPayload(Encoding.UTF8.GetBytes(response))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            mqttClient.PublishAsync(mqttMessage).Wait();
            Console.WriteLine($"Response sent: {response} to topic {responseTopic}");
        }

        public void SendResponse(int? branchUID, int? lockerId, string response)
        {
            // set/BranchUID/BrainUID/lockers/{access is allowed or not allowed}
            var responseTopic = $"set/{branchUID}/BrainUID/lockers/{lockerId}";
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic(responseTopic)
                .WithPayload(Encoding.UTF8.GetBytes(response))
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            mqttClient.PublishAsync(mqttMessage).Wait();
            Console.WriteLine($"Response sent: {response} to topic {responseTopic}");
        }

        private async Task SubscribeToBrainTopic(int branchUID, int brainUID)
        {
            // Subscribing to the topic get/{branchUID}/{brainUID}/cards
            var topic = $"get/{branchUID}/{brainUID}/cards";
            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            Console.WriteLine($"Subscribed to topic {topic}");
        }

        private async Task<IMqttClient> InitializeClient()
        {
            var factory = new MqttFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("Server")
                //.WithTcpServer("broker.hivemq.com", 1883)
                .WithTcpServer("2839c2c4ff524480b1631084b1055b89.s1.eu.hivemq.cloud", 8883)
                .WithCredentials("Andresuga", "Andresuga0713.")
                .WithTlsOptions(new MqttClientTlsOptions() { UseTls = true })
                .Build();

            client.ConnectedAsync += async e =>
            {
                Console.WriteLine("Server connected successfully with MQTT Broker.");
                var branchs = cardLockerService.GetAllActiveBranches();
                foreach (var branch in branchs)
                {
                    await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic($"connection/{branch.Id}").Build());
                    Console.WriteLine($"Subscribed to topic connection/{branch.Id}");
                }
                int branchUID = 1;
                int brainUID = cardLockerService.GetBrainsByBranchId(branchUID).FirstOrDefault().Id; //later change to current brain 
                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic($"get/{branchUID}/{brainUID}/cards").Build());
                Console.WriteLine($"Subscribed to topic get/{branchUID}/{brainUID}/cards");

                await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic($"get/{branchUID}/BrainUID/lockers").Build());
                Console.WriteLine("Subscribed to get/BranchUID/BrainUID/lockers");


            };

            client.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"MessageReceived: {message} on topic {topic}");
                HandleRequest(topic, message);
                return Task.CompletedTask;
            };

            try
            {
                var connectResult = await client.ConnectAsync(options).ConfigureAwait(false);
                Console.WriteLine($"{options.ClientId}: Connecting to {options.ClientId} on port {options.Credentials} ...");
                if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    Console.WriteLine($"Connect failed: {connectResult.ReasonString}");
                    Environment.Exit(-1);
                }
                Console.WriteLine("Server connected to MQTT broker.");


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
