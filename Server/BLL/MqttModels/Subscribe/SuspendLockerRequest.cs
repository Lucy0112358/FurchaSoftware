namespace FurchaBLL.MqttModels.Subscribe
{
    internal class SuspendLockerRequest
    {
        public string Type { get; set; }
        public int Number { get; set; }
        public int Status { get; set; }
    }
}
