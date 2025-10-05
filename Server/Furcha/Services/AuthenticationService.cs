using Domain.Configuration;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Models;
using FurchaDAL.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
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
        private readonly furchaContext Db;

        public AuthenticationService(UserRepository userRepository, AdminRepository adminRepository, furchaContext db)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            Db = db;
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

        public void CreateOrUpdateAdmin(CreateAdminRequest request)
        {
            try
            {
                var companyId = Db.Administrators
                    .FirstOrDefault(a => a.Id == request.ModifiedBy)?.CompanyId
                    ?? throw new Exception("Invalid ModifiedBy admin.");

                var dbAdmin = Db.Administrators
                    .FirstOrDefault(a => a.UserId == request.UserId);

                if (dbAdmin != null)
                {
                    dbAdmin.RoleId = request.RoleId;
                    dbAdmin.ModifiedBy = request.ModifiedBy;
                    //   dbAdmin.LastPasswordChangeDate = DateTime.UtcNow;

                    Db.AdminPermissions.RemoveRange(Db.AdminPermissions.Where(p => p.AdministratorId == dbAdmin.Id));

                    foreach (var permissionId in request.Permissions)
                    {
                        Db.AdminPermissions.Add(new AdminPermission
                        {
                            AdministratorId = dbAdmin.Id,
                            PermissionId = permissionId
                        });
                    }

                    Db.AdminBranches.RemoveRange(Db.AdminBranches.Where(p => p.AdministratorId == dbAdmin.Id));

                    var branchIds = Db.LockerGroups.Where(x => request.GroupIds.Contains(x.Id)).Select(x => x.BranchId.Value).Distinct().ToList();

                    foreach (var branchId in branchIds)
                    {
                        Db.AdminBranches.Add(new FurchaDAL.Models.AdminBranch
                        {
                            AdministratorId = dbAdmin.Id,
                            BranchId = branchId
                        });
                    }

                    Db.AdminLockerGroups.RemoveRange(Db.AdminLockerGroups.Where(p => p.AdminId == dbAdmin.Id));

                    foreach (var lockerGroup in request.GroupIds)
                    {
                        Db.AdminLockerGroups.Add(new FurchaDAL.Models.AdminLockerGroup
                        {
                            AdminId = dbAdmin.Id,
                            LockerGroupId = lockerGroup
                        });
                    }
                }
                else
                {
                    var admin = new FurchaDAL.Models.Administrator
                    {
                        UserId = request.UserId,
                        PasswordHash = "b33f9a399e11b3c36f3d3b338668c41214d8256a95594664166af9fb8b9b72d5",
                        Salt = "furcha-salt",
                        RoleId = request.RoleId,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = null,
                        ModifiedBy = request.ModifiedBy,
                        LastPasswordChangeDate = DateTime.UtcNow,
                        ForcePasswordReset = true,
                        CompanyId = companyId
                    };

                    Db.Administrators.Add(admin);
                    Db.SaveChanges();

                    var branchIds = Db.LockerGroups.Where(x => request.GroupIds.Contains(x.Id)).Select(x => x.BranchId.Value).Distinct().ToList();

                    foreach (var branchId in branchIds)
                    {
                        Db.AdminBranches.Add(new FurchaDAL.Models.AdminBranch
                        {
                            AdministratorId = dbAdmin.Id,
                            BranchId = branchId
                        });
                    }

                    foreach (var permissionId in request.Permissions)
                    {
                        admin.AdminPermissions.Add(new AdminPermission
                        {
                            PermissionId = permissionId
                        });
                    }

                    Db.AdminLockerGroups.RemoveRange(Db.AdminLockerGroups.Where(p => p.AdminId == dbAdmin.Id));

                    foreach (var lockerGroup in request.GroupIds)
                    {
                        Db.AdminLockerGroups.Add(new FurchaDAL.Models.AdminLockerGroup
                        {
                            AdminId = dbAdmin.Id,
                            LockerGroupId = lockerGroup
                        });
                    }
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public List<AdminResult> GetCompanyAdmins(int adminId, string name, int? filterByBranchId = null)
        {
            var companyId = Db.Administrators.First(x => x.Id == adminId).CompanyId;
            var admins = Db.Administrators
                  .Include(a => a.User)
                  .Include(a => a.AdminBranches)
                  .Where(a => a.CompanyId == companyId
                           && a.AdminBranches.Any(b => b.BranchId == filterByBranchId)
                           && (string.IsNullOrEmpty(name)
                               || a.User.Name.Contains(name)
                               || a.User.Surname.Contains(name)))
                  .OrderByDescending(a => a.Id)
                  .ToList();
            // _adminRepository.GetAdminsByCompanyId(companyId);
            var result = new List<AdminResult>();

            foreach (var admin in admins)
            {
                var user = Db.Administrators
                    .Where(a => a.Id == admin.Id)
                    .Select(a => a.User)
                    .FirstOrDefault();  //_userRepository.GetUserByAdminId(admin.Id);

                var adminBranches = Db.AdminBranches
                    .Where(ab => ab.AdministratorId == admin.Id)
                    .Select(ab => ab.Branch)
                    .ToList();  //_userRepository.GetAdminBranchesByAdminId(admin.Id);

                var adminResult = new AdminResult
                {
                    Id = admin.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Branches = adminBranches.Select(b => b.Name).ToList(),
                    IsActive = admin.IsActive,
                    Role = Db.Roles.Where(r => (int)r.Id == admin.RoleId).FirstOrDefault().Name,
                };

                result.Add(adminResult);
            }

            return result;
        }

        public List<PermissionResult> GetRolePermissions(long roleId)
        {
            var permissions = Db.RolePermissions.Include(a => a.Role).Include(p => p.Permission)
        .Where(rp => rp.RoleId == roleId)
        .Select(rp => new RolePermissionResult
        {
            Id = rp.Permission.Id,
            Name = rp.Permission.Name,
            Description = rp.Permission.Description,
            ObjectTypeId = rp.Permission.ObjectTypeId,
            /*  IsOptional = rp.IsOptional*/
        })
        .ToList(); // _userRepository.GetRolePermissions(roleId);
            var objectTypes = Db.ObjectTypes.ToList(); // _userRepository.GetAllObjectTypes();

            var grouped = permissions
                .GroupBy(p => p.ObjectTypeId)
                .Select(group =>
                {
                    var type = objectTypes.FirstOrDefault(t => t.Id == group.Key);
                    return new PermissionResult
                    {
                        TypeId = (int)group.Key,
                        TypeName = type?.Name ?? "Unknown",
                        Permissions = group.ToList()
                    };
                })
                .ToList();

            return grouped;
        }

        public List<Role> GetRoles()
        {
            return Db.Roles.ToList();//_adminRepository.GetRoles();
        }

        public LoginResult LoginToGetJwtToken(AuthenticateRequest authenticateRequest)
        {
            // test authenticateRequest.email = null case with Swagger
            var adminUser = Db.Users.Where(a => a.Email == authenticateRequest.Email).First(); //_userRepository.GetAdminByEmail(authenticateRequest.Email);

            var admin = Db.Administrators.FirstOrDefault(a => a.UserId == adminUser.Id); //_userRepository.GetAdminByUserId(adminUser.Id);

            if (adminUser == null || admin?.Salt == null || admin?.PasswordHash == null || authenticateRequest?.Password == null)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }

            var a = new AdminDto();

            a.Id = admin.Id;
            a.Name = adminUser.Name;
            a.Surname = adminUser.Surname; ;
            a.Role = admin.RoleId.ToString();
            a.Email = adminUser.Email;
            a.CompanyId = adminUser.CompanyId.ToString();
            var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);
            // var hashedPassword = authenticateRequest.Password;

            // var hashedPassword = EncodePassword(password: authenticateRequest.Password, salt: admin.Salt);

            if (hashedPassword != admin.PasswordHash)
            {
                throw new BaseException(ErrorCodeEnum.WrongUsernameOrPassword);
            }
            var loginResult = GetLoginResult(a);

            return loginResult;
        }

        private LoginResult GetLoginResult(AdminDto administrator)
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

        private string GenerateJwtToken(AdminDto admin)
        {
            var rol = Db.Administrators.Where(a => a.Id == admin.Id).FirstOrDefault().RoleId;
            var tokenHandler = new JwtSecurityTokenHandler();
            var permissionNames = Db.RolePermissions
        .Where(rp => rp.RoleId == rol)
        .Select(rp => new RolePermissionResult
        {
            Id = rp.Permission.Id,
            Name = rp.Permission.Name,
            Description = rp.Permission.Description,
            /*  IsOptional = rp.IsOptional,
              ObjectTypeId = rp.ObjectTypeId*/
        })
        .ToList()
                    //_userRepository
                    //.GetRolePermissions((long)admin.Role)
                    .Select(p => p.Name)
                    .ToList();

            var permissionsJson = System.Text.Json.JsonSerializer.Serialize(permissionNames);

            if (string.IsNullOrEmpty(EncryptionSettings.EncryptionKey))
            {
                throw new ArgumentNullException(nameof(EncryptionSettings.EncryptionKey));
            }

            var adminDetails = Db.Administrators.Where(a => a.Id == admin.Id).FirstOrDefault(); //_userRepository.GetAdminById(admin.Id);

            var key = Encoding.ASCII.GetBytes(EncryptionSettings.EncryptionKey);
            var roles = Db.Roles.ToList(); //_adminRepository.GetRoles().ToList();
            var r = roles.Where(r => (long)r.Id == rol).FirstOrDefault().Name;
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, admin.Name),
            new Claim(ClaimTypes.CompanyId, admin.CompanyId.ToString()),
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

        public ShowAdminResult GetAdminById(int id)
        {
            var dbAdmin = Db.Administrators.Include(a => a.User).Include(u => u.Role)
                .Include(a => a.AdminBranches).ThenInclude(b => b.Branch).ThenInclude(x => x.BrainModules)
                .Include(a => a.AdminPermissions).ThenInclude(p => p.Permission).FirstOrDefault(a => a.Id == id);

            return new ShowAdminResult
            {
                Id = dbAdmin.Id,
                RoleId = dbAdmin.RoleId,
                Role = dbAdmin.Role.Name,
                Name = dbAdmin.User.Name,
                Surname = dbAdmin.User.Surname,
                IsActive = dbAdmin.IsActive,
                Permissions = GetRolePermissions((long)dbAdmin.RoleId),
                Branches = dbAdmin.AdminBranches.Select(b => new AdminBranchResult
                {
                    BranchId = b.BranchId,
                    BranchName = b.Branch.Name,
                    GroupIds =Db.AdminLockerGroups.Where(b => b.AdminId == id).Select(x => x.LockerGroupId).ToList(),
                }).ToList()
            };
        }

        public bool DeleteAdmins(List<int> ids)
        {
            var admins = Db.Administrators.Where(a => ids.Contains(a.Id)).ToList();

            if (admins.Any())
            {
                Db.Administrators.RemoveRange(admins);
                Db.SaveChanges();
            }

            return true;
        }

        public void SetAdminState(List<int> ids, int state)
        {
            var admins = Db.Administrators.Where(a => ids.Contains(a.Id)).ToList();

            foreach (var admin in admins)
            {
                admin.IsActive = state == 1 ? true : false;
            }

            Db.SaveChanges();
        }

        public List<LockerType> GetLockerTypes(int adminId)
        {
            var companyId = Db.Administrators.Where(a => a.Id == adminId).FirstOrDefault().CompanyId;

            var company = Db.Companies.FirstOrDefault(c => c.Id == companyId);

            if (company == null || string.IsNullOrWhiteSpace(company.LockerTypeIds))
                return new List<LockerType>();

            // Clean and parse the LockerTypeIds string: "[1, 2, 3]" → ["1","2","3"]
            var cleanedIds = company.LockerTypeIds
                .Trim('[', ']', ' ')                 // remove brackets and spaces
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => id.Trim())             // remove spaces around numbers
                .Select(int.Parse)                   // convert to integers
                .ToList();

            var list = Db.LockerTypes
                .Where(x => cleanedIds.Contains(x.Id))
                .ToList();

            return list;
        }

    }
}
