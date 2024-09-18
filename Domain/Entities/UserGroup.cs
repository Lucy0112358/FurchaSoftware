using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("UserGroup", Schema = "furcha")]
    public class UserGroup
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual Branch Branch { get; set; }

        public virtual ICollection<User_UserGroup> Users { get; set; }
    }
}
