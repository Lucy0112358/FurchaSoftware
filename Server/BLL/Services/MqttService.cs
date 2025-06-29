using Domain.Enums;
using FurchaBLL.Constants;
using FurchaBLL.MqttModels.Subscribe;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Text.Json;

namespace BLL.Services
{
    public class MqttService
    {
        public async Task InitializeClient()
        {
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
              .WithClientId(Constants.MqttClientId)
              .WithTcpServer(Constants.MqttServer, Constants.MqttPort)
              .WithCredentials(Constants.MqttUsername, Constants.MqttPassword)
              .WithTlsOptions(new MqttClientTlsOptions() { UseTls = false })
              .Build();

            mqttClient.ConnectedAsync += async e =>
            {
                await mqttClient.SubscribeAsync("$CONTROL/dynamic-security/#");
                await mqttClient.SubscribeAsync("$SYS/broker/clients/connected");
                await mqttClient.SubscribeAsync("webserver/#");
                await mqttClient.SubscribeAsync("server/status/will");
                var topic = "controller/status/will";
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes("{\"Status\":\"Online\"}"))
                    .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await mqttClient.PublishAsync(mqttMessage);
            };

            mqttClient.ApplicationMessageReceivedAsync += ApplicationMessageReceivedHandler;

            try
            {
                var connectResult = await mqttClient.ConnectAsync(options).ConfigureAwait(false);

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

        private async Task ApplicationMessageReceivedHandler(MqttApplicationMessageReceivedEventArgs e)
        {
            var topic = e.ApplicationMessage.Topic;
            var responseMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
            var message = JsonSerializer.Deserialize<MqttBaseRequest<object>>(responseMessage);

            if (topic.StartsWith("webserver/"))
            {
                if (message.Command == (int)CommandTypes.OpenLocker)
                {
                    // HandleRequest(topic, message);
                }
            }

        }

        public void HandleRequest<T>(string topic, string message)
        {
            var request = new MqttBaseRequest<T>();


        }


    }
}
