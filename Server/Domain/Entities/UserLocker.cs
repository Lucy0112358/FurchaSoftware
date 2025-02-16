using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    [Table("UserLocker", Schema = "furcha")]
    public class UserLocker
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int LockerId { get; set; }

        [Required]
        public int UserId { get; set; }

/*        [ForeignKey("LockerId")]
        public virtual Locker Locker { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }*/
    }
}