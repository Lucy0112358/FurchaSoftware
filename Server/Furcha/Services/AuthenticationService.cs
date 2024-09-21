using Domain.Configuration;
using Domain.Entities;
using Domain.Enums;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using Microsoft.IdentityModel.Tokens;
using MqttService.Application.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            return hashedPassword;
        }
        private LoginResult GetLoginResult(Administrator administrator)
        {
            //don't forget to add the log table data here as well


            return new LoginResult("AdminId");
        
        }

        private string GenerateJwtToken(Administrator admin, RoleEnum adminRole)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrEmpty(EncryptionSettings.EncryptionKey))
            {
                throw new ArgumentNullException(nameof(EncryptionSettings.EncryptionKey));
            }

            var adminDetails = _userRepository.GetAdminById(admin.Id);

            var key = Encoding.ASCII.GetBytes(EncryptionSettings.EncryptionKey);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim("AdminId", admin.Id.ToString()),
            new Claim(ClaimTypes.Role, adminRole.ToString()),
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
