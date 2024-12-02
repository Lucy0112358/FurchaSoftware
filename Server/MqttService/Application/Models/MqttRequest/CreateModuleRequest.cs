using Domain.Enums;

namespace MqttService.Application.Models.MqttRequest
{
    public class CreateModuleRequest
    {
        public LockerTypeEnum? LockerType { get; set; } = null;
        public int BranchId { get; set; }
        public int MaxNumber { get; set; }
        public int MinNumber { get; set; }
        public string MacAddress { get; set; }
        public int? LockerGroupId { get; set; } = null;
    }
}
