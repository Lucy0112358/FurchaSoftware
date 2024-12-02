using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public string MacAddress { get; set; }

        [Column]
        public string IpAddress { get; set; } = "default";

    }
}
