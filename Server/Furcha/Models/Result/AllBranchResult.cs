namespace FurchaAdminApi.Models.Result
{
    public class AllBranchResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public List<int?> LockerTypes { get; set; }

        public int LockersCount { get; set; }

        public int Mode { get; set; }
    }
}
