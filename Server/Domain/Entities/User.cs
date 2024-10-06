using Domain.Enums;
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

        /// <summary>
        /// Email and CompanyId together are under the unique check constraint
        /// because the same person with the same email can be a costomer in different companies
        /// </summary>
        public int CompanyId { get; set; }
        public RoleEnum Role { get; set; }

        public string CountryCode { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
        public StateEnum State { get; set; }

        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<UserLocker> UserLockers { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<UserBranch> UserBranches { get; set; }
        public virtual ICollection<UserEventLogs> UserEventLogs { get; set; }

        public User()
        {
            UserLockers = new HashSet<UserLocker>();
            UserGroups = new HashSet<UserGroup>();
            UserBranches = new HashSet<UserBranch>();
            UserEventLogs = new HashSet<UserEventLogs>();
        }
    }
}
