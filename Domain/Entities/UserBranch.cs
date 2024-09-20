using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("UserBranch", Schema = "furcha")]
    public class UserBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [ForeignKey("User")]
        public int UserId { get; set; } 

        [ForeignKey("Branch")]
        public int BranchId { get; set; } 

        public virtual User User { get; set; }
        public virtual Branch Branch { get; set; }
    }
}
