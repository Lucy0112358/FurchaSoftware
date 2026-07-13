using System.Text.Json.Serialization;

namespace FurchaBLL.MqttModels.Subscribe
{
    public class AssigningUserGroupToLockerRequest
    {
        [JsonPropertyName("entity")]
        public string Entity { get; set; } = "UserGroup";

        [JsonPropertyName("groupExternalId")]
        public long GroupExternalId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("state")]
        public long State { get; set; }

        [JsonPropertyName("activeFrom")]
        public DateOnly? ActiveFrom { get; set; }

        [JsonPropertyName("activeTo")]
        public DateOnly? ActiveTo { get; set; }

        [JsonPropertyName("lockers")]
        public long[] Lockers { get; set; }

        [JsonPropertyName("doors")]
        public long[] Doors { get; set; }

        [JsonPropertyName("memberExternalIds")]
        public int[] MemberExternalIds { get; set; }
    }
}
