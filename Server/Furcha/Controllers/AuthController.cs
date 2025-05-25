using Domain.Attributes;
using Domain.Configuration;
using Domain.Entities;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly AuthenticationService authenticationService;
        public AuthController(AuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpGet("getAuthUser")]
        public IActionResult GetProfile()
        {
            var userId = GetClaimValue("AdminId");
            var email = GetClaimValue("Email");
            var name = GetClaimValue("Name");
            var uname = GetClaimValue("UName");
            var role = GetClaimValue("URole");
            var permissionsJson = User.FindFirst("Permissions")?.Value;
            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID claim is missing");
            }

            return Ok(new { UserId = userId, Email = email, Name = uname, Role = role, Permissions = permissions});
        }

        [Authorize]
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = authenticationService.GetRoles();
            return Ok(roles);
        }

        [Authorize]
        [RequiresPermission("ManageAdmins")]
        [HttpGet("role-permissions")]
        public IActionResult GetRolePermissions(long roleId)
        {
            var permissions = authenticationService.GetRolePermissions(roleId);
            return Ok(permissions);
        }

        [Authorize]
        [RequiresPermission("ManageAdmins")]        
        [HttpPost("create-admin")]
        public ActionResult<ApiResult<Administrators>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                var newAdmin = authenticationService.CreateAdmin(request);
                return Ok(ApiResult<Administrators>.Success(newAdmin));
            }
            catch (Exception ex)
            {
               // return Ok(ApiResult<Administrators>.Success());
                  return NotFound(ApiResult<Administrators>.ErrorResult(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("get-admins")]
        public ActionResult<ApiResult<List<AdminResult>>> GetAdmins()
        {
            var companyId = 5;
            var admins = authenticationService.GetCompanyAdmins(companyId);

            if (admins == null || !admins.Any())
            {
                return NotFound(ApiResult<List<AdminResult>>.ErrorResult("No admins found for the provided company ID."));
            }

            return Ok(ApiResult<List<AdminResult>>.Success(admins));
        }

        [HttpPost("login")]
        public LoginResult Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            var authUser = authenticationService.LoginToGetJwtToken(authenticateRequest);

            return authUser;
        }
    }
}
