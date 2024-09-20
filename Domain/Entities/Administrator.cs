using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Administrators", Schema = "furcha")]
    public class Administrator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Surname { get; set; }

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

        public DateTime? LastPasswordChangeDate { get; set; }

        public bool ForcePasswordReset { get; set; } = false;

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        [ForeignKey("ModifiedBy")]
        public virtual Administrator ModifiedByAdmin { get; set; }
    }
}
