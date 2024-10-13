using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    [Table("User_UserGroup", Schema = "furcha")]
    public class User_UserGroup
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("UserGroup")]
        public int UserGroupId { get; set; }
    }
}
