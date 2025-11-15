using FurchaDAL.Models;

namespace FurchaAdminApi.Models.Result
{
    public class LockersResult
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<LockerWithUsers> GroupLockers { get; set; }
    }

    public class LockerWithUsers
    {
        public int Id { get; set; }

        public long number { get; set; }

        public int? groupid { get; set; }

        public LockerType LockerType { get; set; }

        public int IsActive { get; set; } // active / suspended

        public int Status { get; set; } // free / occcupied

        public int IsOpen { get; set; } // door state

        public int BranchId { get; set; }

        public string PasswordHash { get; set; }

        public List<string> Users { get; set; }
    }
}