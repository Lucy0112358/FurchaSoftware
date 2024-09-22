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

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<UserCard> UserCards { get; set; }
        public virtual ICollection<UserLocker> UserLockers { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<UserBranch> UserBranches { get; set; }
        public virtual ICollection<UserEventLogs> UserEventLogs { get; set; }

        public User()
        {
            UserCards = new HashSet<UserCard>();
            UserLockers = new HashSet<UserLocker>();
            UserGroups = new HashSet<UserGroup>();
            UserBranches = new HashSet<UserBranch>();
            UserEventLogs = new HashSet<UserEventLogs>();
        }
    }
}
