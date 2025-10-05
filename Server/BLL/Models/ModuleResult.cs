namespace FurchaBLL.Models
{
    public class ModuleResult
    {
        public int? LockerType { get; set; }

        public int? LockerGroupId { get; set; }

        public LockerRange LockerRange { get; set; }
    }

    public class LockerRange
    {
        public int Start { get; set; }

        public int End { get; set; }
    }
}