using Domain.Enums;

namespace Domain.Entities
{
    public class LockerGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; } = "";

        public int BranchId { get; set; }

        public string State = "state";
    }
}