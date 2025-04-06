namespace FurchaAdminApi.Models.Request
{
    public class ModuleRequest
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int? LockerGroupId { get; set; }
        public int FirstLocker { get; set; }
        public int LastLocker { get; set; }
        public bool StartBegin { get; set; }       


    }
}