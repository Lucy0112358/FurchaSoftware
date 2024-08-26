using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;

namespace MqttService.Entities
{
    [Table("Card", Schema = "public")]
    public class Card
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int UserId { get; set; }

        [InverseProperty("LockerCard")]
        public IEnumerable<LockerCard> LockerCards { get; set; }

/*        [ForeignKey("UserId")]
        public virtual User User { get; set; }*/
    }
}
