using Domain.Entities;
using Domain.Enums;

namespace FurchaAdminApi.Models.Result
{
    public class UserGroupResult
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<Locker> PermittedLockers { get; set; } 
        public string Branch { get; set; } 
        public StateEnum State { get; set; }
    }
}
