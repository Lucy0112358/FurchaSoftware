using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("User", Schema = "furcha")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [MaxLength(5)]
        public string CountryCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public ICollection<Card> Cards { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<UserLocker> UserLockers { get; set; }
        public ICollection<UserBranch> UserBranches { get; set; }

        public User()
        {
            Cards = new HashSet<Card>();
            UserGroups = new HashSet<UserGroup>();
            UserLockers = new HashSet<UserLocker>();
            UserBranches = new HashSet<UserBranch>();
        }
    }
}
