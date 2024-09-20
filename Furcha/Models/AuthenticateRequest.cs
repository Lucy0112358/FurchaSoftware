namespace FurchaAdminApi.Models
{
    public class AuthenticateRequest
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public AuthenticateRequest()
        {

        }

        public AuthenticateRequest(string? email, string? password)
        {
            Email = email;
            Password = password;
        }
    }
}
