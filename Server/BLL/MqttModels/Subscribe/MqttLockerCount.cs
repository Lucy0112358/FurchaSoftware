namespace FurchaBLL.MqttModels.Subscribe
{
    class MqttLAddLockersInput
    {
        public string BrainUid { get; set; }

        public List<BrainLockers> Lockers { get; set; }
    }

    class BrainLockers
    {
        public int ReaderGroupId { get; set; }

        public int ExternalIds { get; set; }
    }
}
