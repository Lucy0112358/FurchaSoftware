namespace Domain.Configuration
{
    public partial class ClaimTypes
    {
        public const string AdminId = "AdminId";
        public const string Role = System.Security.Claims.ClaimTypes.Role; // required, otherwise role based authentication won't work

        public const string Name = System.Security.Claims.ClaimTypes.Name;
    }
}
