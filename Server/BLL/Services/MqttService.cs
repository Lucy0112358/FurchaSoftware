using Domain.Entities;
using FurchaBLL.Constants;
using FurchaBLL.MqttModels.Publish;
using FurchaBLL.MqttModels.Subscribe;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using System.Text.Json;

namespace BLL.Services
{
    public class MqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        // inject dbcontext

        public MqttService(IMqttClient mqttClient, MqttClientOptions mqttOptions)
        {
            _mqttClient = mqttClient;
            _mqttOptions = mqttOptions;
        }

        public async Task InitializeClient()
        {

            _mqttClient.ConnectedAsync += async e =>
            {
                await _mqttClient.SubscribeAsync("$CONTROL/dynamic-security/#");
                await _mqttClient.SubscribeAsync("$SYS/broker/clients/connected");
                await _mqttClient.SubscribeAsync("webserver/#");
                await _mqttClient.SubscribeAsync("server/status/will");
                var topic = "controller/status/will";
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes("{\"Status\":\"Online\"}"))
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
            };

            _mqttClient.ApplicationMessageReceivedAsync += HandleRequest;

            try
            {
                var connectResult = await _mqttClient.ConnectAsync(_mqttOptions).ConfigureAwait(false);

                if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
                {
                    Environment.Exit(-1);
                }
            }
            catch (Exception ex)
            {
                Environment.Exit(-1);
            }
        }

        private async Task HandleRequest(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var responseMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var message = JsonSerializer.Deserialize<MqttBaseRequest<object>>(responseMessage);

            if (topic.StartsWith("webserver/"))
            {
                if (message.Command == (int)CommandTypes.OpenLocker)
                {
                    var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<Locker>>(responseMessage);

                    // db update
                }
                if (message.Command == (int)CommandTypes.OpenLockersFromAdmin)
                {
                    var lockersPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<int>>>(responseMessage);
                    // take lockers id-s
                    // iterate through them 
                    // save them in the db
                }
                if (message.Command == (int)CommandTypes.CreateUserFromAdmin)
                {
                    var userPayload = JsonSerializer.Deserialize<MqttBaseRequest<List<Locker>>>(responseMessage);

                    // take user and mark if is added
                }
            }

        }

        public async Task CompanyRegistration(AddCompanyMqtt company)
        {
            var payload = JsonSerializer.Serialize(company);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("$CONTROL/dynamic-security/v1")
                .WithPayload(payload)
                .WithRetainFlag(true)
                .Build();

            var result = await _mqttClient.PublishAsync(message, CancellationToken.None);
        }


/*        public async Task AddAutorazationMqttClientAsync(Guid accountUID, string brainPass)
        {

            var createClientCommand = new MqttBaseRequest<AddCompanyMqtt>
            {
                Username = accountUID.ToString(),
                Password = brainPass,
                Roles = new List<CreateClientCommand.Role>
                    {
                        new CreateClientCommand.Role { RoleName = "user", Priority = 1 }
                    }
            };
            string batch_number = "1";
            await SendCommandAsync(createClientCommand);

        }*/
    }
}