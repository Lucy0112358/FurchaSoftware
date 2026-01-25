using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    [Table("Locker", Schema = "furcha")]
    public class Locker
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int BrainId { get; set; }

        [Column]
        public int number { get; set; }

        [Column]
        public int? groupid { get; set; }

        [Column]
        public string LockerType { get; set; }

        [Column]
        public int IsActive { get; set; }

        [Column]
        public int IsOpen { get; set; }

        //[Column]
        //public int Status { get; set; }

        [Column]
        public int BranchId { get; set; }

        [Column]
        public string PasswordHash { get; set; }


        //[JsonIgnore]

        //[InverseProperty("LockerCard")]
        //public IEnumerable<UserLocker> UserLocker { get; set; }

    }
}
