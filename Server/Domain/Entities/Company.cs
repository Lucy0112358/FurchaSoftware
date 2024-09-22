namespace Domain.Entities
{
    public class Company
    {
        public int Id { get; set; } 

        public string Name { get; set; } 

        public string City { get; set; } 

        public string Street { get; set; } 

        public int Country { get; set; } 

        public string Email { get; set; } 

        public string Phone { get; set; } 

        public string CountryCode { get; set; } 

        public ICollection<Branch> Branches { get; set; }

        public Company()
        {
            Branches = new List<Branch>();
        }
    }

}
