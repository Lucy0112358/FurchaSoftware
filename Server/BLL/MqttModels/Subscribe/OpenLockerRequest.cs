namespace FurchaBLL.MqttModels.Subscribe
{
    public class OpenLockerRequest
    {
        public string Type {  get; set; }   // change to int in db
        public int Status {  get; set; }   // change to int in db
        
        public int Number { get; set; }

    }
    public class LockerData
    {
        public int Count { get; set; }
        public string Type { get; set; } 
        public List<int?> Ids { get; set; } 
    }
}
