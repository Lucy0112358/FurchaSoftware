using Domain.Enums;

namespace MqttService.Application.Models.MqttRequest
{
    public class CreateModuleRequest
    {
        public locker_type LockerType { get; set; } = locker_type.Common;
        public int BranchId { get; set; }
        public int LastLocker { get; set; }
        public int FirstLocker { get; set; }
        public string MacAddress { get; set; }
        public int? LockerGroupId { get; set; } = null;
    }
}
