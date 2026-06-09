

namespace FurchaBLL.MqttModels.Subscribe
{
    public class AssigningUserToLockerRequest
    {
        public long UserId { get; set; }
        public long[] Lockers { get; set; }
        public long[] Doors { get; set; }
        public string Action { get; set; }
    }
}
