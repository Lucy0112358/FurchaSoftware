using FurchaDAL.Models;

namespace FurchaBLL.MqttModels.Subscribe
{
    internal class LockerModeChangeRequest
    {
        public string Type { get; set; }
        public int Number { get; set; }
        public LockerMode Mode { get; set; }
        public DateTime Date { get; set; }
    }
}
