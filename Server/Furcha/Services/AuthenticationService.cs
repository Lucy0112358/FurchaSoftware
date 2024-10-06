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
using ClaimTypes = Domain.Configuration.ClaimTypes;

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
        public LoginResult LoginToGetJwtToken(AuthenticateRequest authenticateRequest)
        {
            // test authenticateRequest.email = null case with Swagger
            var adminUser = _userRepository.GetAdminByEmail(authenticateRequest.Email);
            var admin = _userRepository.GetAdminByUserId(adminUser.Id);

            if (adminUser == null || admin?.Salt == null || admin?.PasswordHash == null || authenticateRequest?.Password == null)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }

            var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);
          //  var hashedPassword = authenticateRequest.Password;

            if (hashedPassword != admin.PasswordHash)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }
            var loginResult = GetLoginResult(admin);

            return loginResult;
        }
        private LoginResult GetLoginResult(Administrators administrator)
        {
            //don't forget to add the log table data here as well
            var jwtToken = GenerateJwtToken(administrator);

            return new LoginResult(jwtToken)
            {
                Id = administrator.Id,
                Name = administrator.Name,
                Surname = administrator.Surname,
                Role = administrator.Role,
            };
        }

        private string GenerateJwtToken(Administrators admin)
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
            new Claim(ClaimTypes.AdminId, admin.Id.ToString()),
            new Claim(ClaimTypes.Role, admin.Role.ToString()),
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
