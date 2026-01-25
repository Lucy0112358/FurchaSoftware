using Domain.Attributes;
using Domain.Configuration;
using FurchaAdminApi.Mappers.Company;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using FurchaBLL.Services;
using FurchaDAL.Models;
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
        private readonly CompanyService _companyService;

        public AuthController(AuthenticationService authenticationService, CompanyService companyService)
        {
            this.authenticationService = authenticationService;
            _companyService = companyService;
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

            List<string>? permissions = null;
            if (!string.IsNullOrEmpty(permissionsJson))
            {
                permissions = JsonSerializer.Deserialize<List<string>>(permissionsJson);
            }
            else
            {
                permissions = new List<string>(); // or leave null if you prefer
            }

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID claim is missing");
            }

            return Ok(new { UserId = userId, Email = email, Name = uname, Role = role, Permissions = permissions });
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
        /*
                [Authorize]
                [RequiresPermission("ManageAdmins")]       */
        [HttpPost("create-admin")]
        public ActionResult<ApiResult<Administrator>> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                var adminId = GetClaimValue("AdminId");
                request.ModifiedBy = int.Parse(adminId);
                authenticationService.CreateOrUpdateAdmin(request);
                return Ok(ApiResult<Administrator>.Success());
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<Administrator>.ErrorResult(ex.Message));
            }
        }

        [HttpPost("edit-admin")]
        public ActionResult<ApiResult<Administrator>> EditAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                var adminId = GetClaimValue("AdminId");
                request.ModifiedBy = int.Parse(adminId);
                authenticationService.CreateOrUpdateAdmin(request);
                return Ok(ApiResult<Administrator>.Success());
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<Administrator>.ErrorResult(ex.Message));
            }
        }

        /*
                [Authorize]*/
        [HttpGet("get-admins")]
        public ActionResult<ApiResult<List<AdminResult>>> GetAdmins([FromQuery] string? name, [FromQuery] int? branchId = null)
        {
            var adminId = GetClaimValue("AdminId");
            var admins = authenticationService.GetCompanyAdmins(int.Parse(adminId), name, branchId);

            if (admins == null || !admins.Any())
            {
                return Ok(ApiResult<List<AdminResult>>.ErrorResult("No admins found for the provided company ID."));
            }

            return Ok(ApiResult<List<AdminResult>>.Success(admins));
        }

        [HttpPost("login")]
        public LoginResult Login([FromBody] AuthenticateRequest authenticateRequest)
        {
            var authUser = authenticationService.LoginToGetJwtToken(authenticateRequest);

            return authUser;
        }

        [HttpGet("get-admin-by-id")]
        public ActionResult<ApiResult<ShowAdminResult>> GetAdminById([FromQuery] int id)
        {
            var admin = authenticationService.GetAdminById(id);

            return Ok(ApiResult<ShowAdminResult>.Success(admin));
        }

        [HttpGet("getLockerTypes")]
        public ActionResult<ApiResult<List<LockerType>>> GetCompanyLockerTypes()
        {
            var adminId = GetClaimValue("AdminId");

            var l = authenticationService.GetLockerTypes(int.Parse(adminId));

            return Ok(ApiResult<List<LockerType>>.Success(l));
        }

        [HttpPost("add-company")]
        public IActionResult RegisterCompany([FromBody] CreateCompanyRequest company)
        {
            try
            {
                _companyService.RegisterCompany(company.ToBllCompany());

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete-admins")]
        public IActionResult DeleteAdmins([FromBody] DeleteAdminRequest request)
        {
            authenticationService.DeleteAdmins(request.Ids);

            return Ok(true);
        }

        [HttpPatch("set-admin-state")]
        public IActionResult SetAdminState([FromBody] ChangeAdminStateRequest request)
        {
            authenticationService.SetAdminState(request.Ids, request.State);

            return Ok();
        }


    }
}
