namespace Domain.Entities
{
    public class Branch
    {
        public int Id { get; set; } 

        public string Name { get; set; } 

        public string Comment { get; set; }

        public int CompanyId { get; set; }

        public int AddressId { get; set; } 

        public int Mode { get; set; }

    }

}
