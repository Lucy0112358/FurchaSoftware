using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Roles", Schema = "furcha")]
    public class Roles
    {
        // We manually set it in the database from RoleEnum values
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public RoleEnum Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(255)]
        public string OpenName { get; set; }
    }
}
