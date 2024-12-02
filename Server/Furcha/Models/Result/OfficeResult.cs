using Domain.Entities;

namespace FurchaAdminApi.Models.Result
{
    public class OfficeResult
    {
        public string OfficeName { get; set; }
        public List<Locker> Lockers { get; set; }
    }
}
