using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;

namespace Domain.Entities
{
    [Table("Card", Schema = "furcha")]
    public class Card
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int UserId { get; set; }

        [InverseProperty("LockerCard")]
        public IEnumerable<UserCard> UserCard { get; set; }

        /*        [ForeignKey("UserId")]
                public virtual User User { get; set; }*/
    }
}
