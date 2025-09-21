namespace FurchaAdminApi.Models.Result
{
    public class SingleUserResult
    {
        public int Id { get; set; }
        public bool IsPinRequired { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public string State { get; set; }
        public List<string> Cards { get; set; }
        public List<int> UserGroups { get; set; }
        public List<BranchResult> Branches { get; set; }
    }
}
