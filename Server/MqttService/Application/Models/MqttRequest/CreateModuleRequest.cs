using Domain.Enums;

namespace MqttService.Application.Models.MqttRequest
{
    public class CreateModuleRequest
    {
        public string LockerType { get; set; } = null;
        public int BranchId { get; set; }
        public int LastLocker { get; set; }
        public int FirstLocker { get; set; }
        public string MacAddress { get; set; }
        public int? LockerGroupId { get; set; } = null;
    }
}
