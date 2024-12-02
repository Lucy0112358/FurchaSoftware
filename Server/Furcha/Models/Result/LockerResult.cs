using Domain.Entities;

namespace FurchaAdminApi.Models.Result
{
    public class LockerResult
    {
        public string GroupName { get; set; }
        public List<Locker> GroupLockers { get; set; }
    }
}

