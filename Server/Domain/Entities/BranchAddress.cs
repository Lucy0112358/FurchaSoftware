namespace Domain.Entities
{
    public class BranchAddress
    {
        public int Id { get; set; } 

        public string Street { get; set; }

        public string City { get; set; } 

        public string PostalCode { get; set; } 

        public string Country { get; set; } 

        public Branch Branch { get; set; } 
    }

}
