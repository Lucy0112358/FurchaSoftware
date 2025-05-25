using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class AdminBranch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Admin")]
        public int AdminId { get; set; }

        [ForeignKey("Branch")]
        public int BranchId { get; set; }

        public virtual Administrator Admin { get; set; }
        public virtual Branch Branch { get; set; }
    }
}