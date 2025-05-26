using Domain.Configuration;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using Microsoft.IdentityModel.Tokens;
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
        private readonly AdminRepository _adminRepository;

        public AuthenticationService(UserRepository userRepository, AdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
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

        public Administrators CreateAdmin(CreateAdminRequest request)
        {         
            // admin branch
            // admin lockers
            var admin = new Administrators
            {
                UserId = request.UserId,
                PasswordHash = "b33f9a399e11b3c36f3d3b338668c41214d8256a95594664166af9fb8b9b72d5",
                Salt = "furcha-salt",
                RoleId = request.RoleId,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = true,
                ModifiedBy = 8,
                LastPasswordChangeDate = DateTime.UtcNow,
                ForcePasswordReset = true,
                   CompanyId = 5,


            };
            admin = _adminRepository.CreateAdmin(admin);
            foreach (var permission in request.Permissions)
            {
                var a = new AdminPermissions
                {
                    AdministratorId = admin.Id,
                    PermissionId = permission
                };
                _adminRepository.CreateAdminPermissions(a);
            }
            return admin;
        }

        public List<AdminResult> GetCompanyAdmins(int companyId)
        {
            var admins = _adminRepository.GetAdminsByCompanyId(companyId);
            var result = new List<AdminResult>();

            foreach (var admin in admins)
            {
                var user = _userRepository.GetUserByAdminId(admin.Id);

                var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

                var adminResult = new AdminResult
                {
                    Id = admin.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Branches = adminBranches.Select(b => b.Name).ToList(),
                    IsActive = admin.IsActive,
                    Role = _adminRepository.GetRoles().Where(r => (int)r.Id == admin.RoleId).FirstOrDefault().Name,
                };

                result.Add(adminResult);
            }

            return result;
        }

        public List<PermissionResult> GetRolePermissions(long roleId)
        {
            var permissions = _userRepository.GetRolePermissions(roleId);
            var objectTypes = _userRepository.GetAllObjectTypes();

            var grouped = permissions
                .GroupBy(p => p.ObjectTypeId)
                .Select(group =>
                {
                    var type = objectTypes.FirstOrDefault(t => t.Id == group.Key);
                    return new PermissionResult
                    {
                        TypeId = group.Key,
                        TypeName = type?.Name ?? "Unknown",
                        Permissions = group.ToList()
                    };
                })
                .ToList();

            return grouped;
        }



        public List<Roles> GetRoles()
        {
            return _adminRepository.GetRoles();
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
            admin.Name = adminUser.Name;
            admin.Role = adminUser.Role;
            admin.Email = adminUser.Email;
            var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);
            // var hashedPassword = authenticateRequest.Password;

            // var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);

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
            var jwtToken = GenerateJwtToken(administrator);

            return new LoginResult(jwtToken)
            {
                Id = administrator.Id,
                Name = administrator.Name,
                Surname = administrator.Surname,
                Role = administrator.Role,
            };
        }

        private string GenerateJwtToken(Administrator admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var permissionNames = _userRepository
                    .GetRolePermissions((long)admin.Role)
                    .Select(p => p.Name)
                    .ToList();

            var permissionsJson = System.Text.Json.JsonSerializer.Serialize(permissionNames);

            if (string.IsNullOrEmpty(EncryptionSettings.EncryptionKey))
            {
                throw new ArgumentNullException(nameof(EncryptionSettings.EncryptionKey));
            }

            var adminDetails = _userRepository.GetAdminById(admin.Id);

            var key = Encoding.ASCII.GetBytes(EncryptionSettings.EncryptionKey);
            var roles = _adminRepository.GetRoles().ToList();
            var r = roles.Where(r => (long)r.Id == (long)admin.Role).FirstOrDefault().Name;
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.UName, admin.Name),
            new Claim(ClaimTypes.Email, admin.Email),
            new Claim(ClaimTypes.AdminId, admin.Id.ToString()),
            new Claim(ClaimTypes.URole, admin.Role.ToString()),
            new Claim("aud", EncryptionSettings.Audience),
            new Claim("iss", EncryptionSettings.Issuer),
               new Claim("Permissions", permissionsJson)
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
