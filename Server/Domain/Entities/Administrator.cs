using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Administrator", Schema = "furcha")]
    public class Administrator : User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }

/*        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Surname { get; set; }*/

        public int RoleId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Salt { get; set; }

        public bool IsActive { get; set; } = true;

        public bool? IsDeleted { get; set; } 

        public int? ModifiedBy { get; set; }

        public int UserId { get; set; }

        public DateTime? LastPasswordChangeDate { get; set; }

        public bool ForcePasswordReset { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        /*        [ForeignKey("ModifiedBy")]
                public virtual Administrator ModifiedByAdmin { get; set; }
                public virtual ICollection<AdminBranch> AdminBranches { get; set; }*/

        /*  public Administrator()
          {
              AdminBranches = new HashSet<AdminBranch>();
          }*/
    }
}
