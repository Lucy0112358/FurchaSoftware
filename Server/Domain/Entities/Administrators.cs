using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Administrators", Schema = "furcha")]
    public class Administrators 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int Id { get; set; }
        public int CompanyId { get; set; }

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

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public long RoleId   { get; set; }

    /*    [ForeignKey("ModifiedBy")]
        public virtual Administrators ModifiedByAdmin { get; set; }*/
      //  public virtual ICollection<AdminBranch> AdminBranches { get; set; }

      /*  public Administrators()
        {
            AdminBranches = new HashSet<AdminBranch>();
        }*/
    }
}
