namespace FurchaAdminApi.Hubs
{
    public static class HubGroupNames
    {
        private const string _companyPrefix = "company";

        public static string GetGroupName(string prefix, int id)
        {
            return $"{prefix}_{id}".ToLowerInvariant();
        }

        public static string GetCompanyGroup(int companyId)
        {
            return GetGroupName(_companyPrefix, companyId);
        }
    }
}
