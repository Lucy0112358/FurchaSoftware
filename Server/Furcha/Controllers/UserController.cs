using Domain.Attributes;
using Domain.Configuration;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly UserService userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet("company-users")]
        public ActionResult<ApiResult<List<UserResult>>> GetAdminUsers()
        {
            var adminId = GetClaimValue("AdminId");
            var httpContext = _httpContextAccessor.HttpContext;
            var userClaims = httpContext?.User.Claims;
            var nameClaim = userClaims?.FirstOrDefault(c => c.Type == "name")?.Value;
            var users = userService.GetUsersForAdminBasedOnRole(int.Parse(adminId));

            if (users == null || !users.Any())
            {
                return Ok(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }

        [Authorize]
        [HttpGet("filtered-users")]
        public ActionResult<ApiResult<List<UserResult>>> GetFilteredUsersWithPagination(
              [FromQuery] int? groupId = null,
              [FromQuery] int? branchId = null,
              [FromQuery] int pageNumber = 1,
              [FromQuery] int page = 100)
        {
            var adminId = GetClaimValue("AdminId");
            var users = userService.GetFilteredUsersByPagination(int.Parse(adminId), groupId, branchId, pageNumber, page);

            if (users == null || !users.Any())
            {
                return Ok(ApiResult<List<UserResult>>.ErrorResult("No filtered users found for the given criteria."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }

        [Authorize]
        [HttpGet("user-groups")]
        public ActionResult<ApiResult<List<UserGroupResult>>> GetUserGroupsByAdminId()
        {
            var adminId = GetClaimValue("AdminId");

            var userGroups = userService.GetUserGroupsForAdminBasedOnRole(int.Parse(adminId));

            if (userGroups == null)
            {
                return Ok(ApiResult<List<UserGroupResult>>.ErrorResult("No user groups found for the current admin permissions"));
            }

            return Ok(ApiResult<List<UserGroupResult>>.Success(userGroups));
        }

        [Authorize]
        [HttpGet("search-user")]
        public ActionResult<ApiResult<List<UserResult>>> SearchUsersOfAdmin([FromQuery] string name)
        {
            var adminId = GetClaimValue("AdminId");

            var users = userService.SearchUsersOfAdmin(name, int.Parse(adminId));

            if (users == null || users.Count == 0)
            {
                return Ok(ApiResult<List<UserResult>>.ErrorResult("No users found for the provided admin or search criteria."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }

        /*  [Authorize]
          [RequiresPermission("CreateUser")]*/
        [HttpPost("add-user")]
        public async Task<ActionResult<ApiResult<UserResult>>> AddUser([FromBody] UserCreateRequest userCreateRequest)
        {
            var adminId = GetClaimValue("AdminId");

            if (userCreateRequest == null)
            {
                return BadRequest(ApiResult<UserResult>.ErrorResult("Invalid user data."));
            }

            var result = userService.AddUser(userCreateRequest, int.Parse(adminId));

            if (result == null)
            {
                return BadRequest(ApiResult<UserResult>.ErrorResult(ErrorCodeEnum.GenericErrorRetry, "User could not be created."));
            }

            return Ok(ApiResult<UserResult>.Success(result));
        }

        [Authorize]
        [RequiresPermission("ManageUserGroup")]
        [HttpPost("add-user-group")]
        public ActionResult<ApiResult<UserGroupResult>> AddUserGroup([FromBody] UserGroupRequest userGroupRequest)
        {
            if (userGroupRequest == null)
            {
                return BadRequest(ApiResult<UserGroupResult>.ErrorResult("Invalid data."));
            }

            var adminId = GetClaimValue("AdminId");

            userGroupRequest.AdminId = int.Parse(adminId);

            try
            {
                var result = userService.AddUserGroup(userGroupRequest);

                if (result == null)
                {
                    return BadRequest(ApiResult<UserGroupResult>.ErrorResult(ErrorCodeEnum.GenericErrorRetry, "Group could not be created."));
                }

                return Ok(ApiResult<UserGroupResult>.Success(result));
            }
            catch (BaseException ex)
            {
                return BadRequest(ApiResult<UserGroupResult>.ErrorResult(ErrorCodeEnum.GenericErrorRetry, "Group could not be created."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResult<UserGroupResult>.ErrorResult("An unexpected error occurred."));
            }
        }


    }
}
