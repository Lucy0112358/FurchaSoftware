using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class BrainModule
    {
        [Column]
        public int Id { get; set; }

        [Column]
        public int? GroupId { get; set; }

        [Column]
        public int BranchId { get; set; }

        [Column]
        public int Status { get; set; }

        [Column]
        public string MacAddress { get; set; }

        [Column]
        public string Info { get; set; }

        [Column]
        public string IpAddress { get; set; } = "default";

    }
}
