using FurchaBLL.MqttModels.Subscribe;
using MQTTnet.Client;

namespace FurchaBLL.Interfaces
{
    /// <summary>
    /// Interface for MQTT service operations including publishing and managing connections.
    /// </summary>
    public interface IMqttService
    {
        /// <summary>
        /// Publishes a typed MQTT message to a specified topic.
        /// </summary>
        /// <typeparam name="T">The type of data in the MQTT request.</typeparam>
        /// <param name="request">The MQTT request containing command and data.</param>
        /// <param name="topic">The MQTT topic to publish to.</param>
        /// <returns>A task representing the asynchronous publish operation.</returns>
        Task PublishAsync<T>(MqttBaseRequest<T> request, string topic);

        /// <summary>
        /// Adds an MQTT account with specified credentials.
        /// </summary>
        /// <param name="accountUID">The unique identifier for the account.</param>
        /// <param name="brainPass">The password for the brain module.</param>
        /// <returns>A task representing the asynchronous account creation operation.</returns>
        Task AddAccountAsync(Guid? accountUID, string brainPass);

        /// <summary>
        /// Checks if the MQTT client is currently connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Registers the message received handler for processing incoming MQTT messages.
        /// </summary>
        /// <param name="handler">The message handler delegate.</param>
        void RegisterMessageHandler(Func<MqttApplicationMessageReceivedEventArgs, Task> handler);

        /// <summary>
        /// Connects to the MQTT broker with automatic retry logic.
        /// </summary>
        /// <returns>A task representing the asynchronous connection operation.</returns>
        Task ConnectAsync();

        /// <summary>
        /// Subscribes to a list of MQTT topics.
        /// </summary>
        /// <param name="topics">The topics to subscribe to.</param>
        /// <returns>A task representing the asynchronous subscription operation.</returns>
        Task SubscribeAsync(params string[] topics);

        /// <summary>
        /// Disconnects from the MQTT broker.
        /// </summary>
        /// <returns>A task representing the asynchronous disconnect operation.</returns>
        Task DisconnectAsync();
    }
}
