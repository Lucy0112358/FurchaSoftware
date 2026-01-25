using Domain.Enums;
using System.Text.Json.Serialization;

namespace MqttService.Application.Models.MqttRequest
{
    public class CreateModuleRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public locker_type LockerType { get; set; }
        public int BranchId { get; set; }
        public int LastLocker { get; set; }
        public int FirstLocker { get; set; }
        public string MacAddress { get; set; }
        public int? LockerGroupId { get; set; } = null;
    }
}
