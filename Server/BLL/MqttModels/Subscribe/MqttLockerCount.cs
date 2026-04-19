namespace FurchaBLL.MqttModels.Subscribe
{
    class MqttLAddLockersInput
    {
        public string BrainUid { get; set; }
        public int ChunkIndex { get; set; }
        public int TotalChunks { get; set; }
        public List<BrainLockers> Lockers { get; set; }
    }

    class BrainLockers
    {
        public int ReaderGroupId { get; set; }

        public int ExternalIds { get; set; }
    }
}
