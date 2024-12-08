
using Domain.Entities;

namespace FurchaAdminApi.Models.Result
{
    public class ModuleResult
    {
        public string OfficeName { get; set; }
        public List<ModuleInfo> Modules { get; set; }
    }

    public class ModuleInfo
    {
        public string GroupName { get; set; }
        public List<BrainModule> GroupModules { get; set; }
    }
}
