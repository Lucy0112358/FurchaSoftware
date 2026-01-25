namespace FurchaAdminApi.Models.Result
{
    public class AdminResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Role { get; set; }

        public List<string> Cards { get; set; }

        public List<string> Branches { get; set; }

        public bool IsActive { get; set; }
    }
}
