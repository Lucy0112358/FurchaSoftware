using Domain.Entities;
using FurchaAdminApi.Models;
using FurchaAdminApi.Repos;
using MqttService.Application.Exceptions;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;

namespace FurchaAdminApi.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _userRepository;
        public AuthenticationService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private string EncodePassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = password + salt;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));

                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public string LoginToGetJwtToken(AuthenticateRequest authenticateRequest)
        {
            // test authenticateRequest.email = null case with Swagger
            var admin = _userRepository.GetAdminByEmail(authenticateRequest.Email);

            if (admin == null || admin?.Salt == null || admin?.PasswordHash == null || authenticateRequest?.Password == null)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }

            var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);

            if (hashedPassword != admin.PasswordHash)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }
            var loginResult = GetLoginResult(admin);
            return loginResult;
        }
        private LoginResult GetLoginResult(Administrator administrator)
        {
            //don't forget to add the log table data here as well

            return new LoginResult();
        }
    }
}
