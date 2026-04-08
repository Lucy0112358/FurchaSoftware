using MQTTnet.Client;

namespace FurchaBLL.Interfaces
{
    /// <summary>
    /// Interface for handling incoming MQTT messages.
    /// </summary>
    public interface IMqttMessageHandler
    {
        /// <summary>
        /// Handles an incoming MQTT message and performs appropriate actions.
        /// </summary>
        /// <param name="eventArgs">The MQTT message event arguments.</param>
        /// <returns>A task representing the asynchronous message handling operation.</returns>
        Task HandleAsync(MqttApplicationMessageReceivedEventArgs eventArgs);
    }
}
