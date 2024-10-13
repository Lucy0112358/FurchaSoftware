using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public string Name { get; set; }

        public string Surname { get; set; }
        public StateEnum State { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}
