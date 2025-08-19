namespace FurchaBLL.Models
{
    public class ModuleResult
    {
        public string LockerType { get; set; }

        public int LockerGroup { get; set; }

        public LockerRange LockerRange { get; set; }
    }

    public class LockerRange
    {
        public int Start { get; set; }

        public int End { get; set; }
    }
}