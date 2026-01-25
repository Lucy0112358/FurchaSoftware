using Domain.Enums;

namespace FurchaAdminApi.Models.Result
{
    public class LoginResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        /// <summary>
        /// Json Web Token (JWT)
        /// </summary>
        public string Token { get; set; } = string.Empty;

        public RoleEnum Role { get; set; }
        public LoginResult(string? token)
        {
            Token = token ?? string.Empty;
        }
    }
}
