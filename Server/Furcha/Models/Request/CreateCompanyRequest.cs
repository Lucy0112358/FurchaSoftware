namespace FurchaAdminApi.Models.Request
{
    public class CreateCompanyRequest
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public int Country { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string CountryCode { get; set; }
    }
}
