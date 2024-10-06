using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    [Table("UserGroup", Schema = "furcha")]
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public StateEnum State { get; set; }

        [JsonIgnore]
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGroup_LockerGroup> UserGroupLockerGroups { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserGroup_Branch> UserGroupBranches { get; set; }
    }
}