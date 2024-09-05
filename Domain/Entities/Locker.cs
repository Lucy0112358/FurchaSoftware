using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Locker", Schema = "public")]
    public class Locker
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int LockerTypeId { get; set; }

        [Column]
        public int IsActive { get; set; }

        [Column]
        public int IsOpen { get; set; }

        [Column]
        public int Status { get; set; }

        [Column]
        public int GroupId { get; set; }

        [Column]
        public string PasswordHash { get; set; }

        [InverseProperty("LockerCard")]
        public IEnumerable<LockerCard> LockerCards { get; set; }

    }
}
