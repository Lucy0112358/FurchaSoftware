using System.Text.Json.Serialization;

namespace FurchaBLL.MqttModels.Subscribe
{
    public partial class AssigningUserToLockerRequest
    {
        [JsonPropertyName("externalId")]
        public long ExternalId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("personalId")]
        public string PersonalId { get; set; }

        [JsonPropertyName("userGroup")]
        public string UserGroup { get; set; }

        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("pin")]
        public long? Pin { get; set; }

        [JsonPropertyName("activeFrom")]
        public DateOnly? ActiveFrom { get; set; }

        [JsonPropertyName("activeTo")]
        public DateOnly? ActiveTo { get; set; }

        [JsonPropertyName("cards")]
        public string[] Cards { get; set; }

        [JsonPropertyName("lockers")]
        public int[] Lockers { get; set; }

        [JsonPropertyName("doors")]
        public long[] Doors { get; set; }

        [JsonPropertyName("rules")]
        public object[] Rules { get; set; }
    }
}
