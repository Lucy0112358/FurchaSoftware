namespace FurchaAdminApi.Models.Request
{
    public class ChangeAdminStateRequest
    {
        public List<int> Ids { get; set; }

        public int State { get; set; }
    }
}
