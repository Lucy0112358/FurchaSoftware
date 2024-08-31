using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("LockerCard", Schema = "public")]
    public class LockerCard
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int LockerId { get; set; }

        [Column]
        public int CardId { get; set; }

        [ForeignKey("Locker")]
        public virtual Locker Locker { get; set; }

        [ForeignKey("Card")]
        public virtual Card Card { get; set; }
    }
}
