namespace FurchaBLL.MqttModels.Subscribe
{
    public class AssigningUserGroupToLockerRequest
    {
        public int GroupId { get; set; }
        public long[] Lockers { get; set; }
        public long[] Doors { get; set; }
        public string Action { get; set; }
    }
}
