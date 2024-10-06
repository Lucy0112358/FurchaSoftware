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

        [Column]
        public string CardNumber { get; set; }

    }
}
