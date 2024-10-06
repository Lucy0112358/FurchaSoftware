using Domain.Configuration;
using Domain.Enums;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("company-users")]
        [AllowAnonymous]
        public ActionResult<ApiResult<List<UserResult>>> GetAdminUsers([FromQuery] int adminId)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var userClaims = httpContext?.User.Claims;
            var nameClaim = userClaims?.FirstOrDefault(c => c.Type == "name")?.Value;
            var users = userService.GetUsersForAdminBasedOnRole(adminId);

            if (users == null || !users.Any())
            {
                return NotFound(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }


        [HttpGet("filtered-users")]
        public ActionResult<ApiResult<List<UserResult>>> GetFilteredUsersWithPagination(
              [FromQuery] int adminId,
              [FromQuery] int? GroupId,
              [FromQuery] int? BranchId,
              [FromQuery] int pageNumber = 1,
              [FromQuery] int page = 10)
        {
            var users = userService.GetFilteredUsersByPagination(adminId, GroupId, BranchId, pageNumber, page);

            if (users == null || !users.Any())
            {
                return NotFound(ApiResult<List<UserResult>>.ErrorResult("No filtered users found for the given criteria."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }


        [HttpGet("user-groups")]
        public ActionResult<ApiResult<List<UserGroupResult>>> GetUserGroupsByAdminId(int adminId)
        {
            var userGroups = userService.GetUserGroupsForAdminBasedOnRole(adminId);

            if (userGroups == null)
            {
                return NotFound(ApiResult<List<UserGroupResult>>.ErrorResult("No user groups found for the current admin permissions"));
            }

            return Ok(ApiResult<List<UserGroupResult>>.Success(userGroups));
        }


        [HttpGet("search-user")]
        public ActionResult<ApiResult<List<UserResult>>> SearchUsersOfAdmin([FromQuery] string name, [FromQuery] int adminId)
        {
            var users = userService.SearchUsersOfAdmin(name, adminId);

            if (users == null || users.Count == 0)
            {
                return Ok(ApiResult<List<UserResult>>.ErrorResult("No users found for the provided admin or search criteria."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }

        [HttpPost("add-user")]
        public ActionResult<ApiResult<UserResult>> AddUser([FromBody] UserCreateRequest userCreateRequest)
        {
            if (userCreateRequest == null)
            {
                return BadRequest(ApiResult<UserResult>.ErrorResult("Invalid user data."));
            }

            var result = userService.AddUser(userCreateRequest);

            if (result == null)
            {
                return BadRequest(ApiResult<UserResult>.ErrorResult(ErrorCodeEnum.GenericErrorRetry, "User could not be created."));
            }

            return Ok(ApiResult<UserResult>.Success(result));
        }


    }
}
