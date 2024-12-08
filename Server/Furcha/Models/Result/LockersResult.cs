using Domain.Entities;

namespace FurchaAdminApi.Models.Result
{
    public class LockersResult
    {
        public string GroupName { get; set; }
        public List<Locker> GroupLockers { get; set; }
    }
}

