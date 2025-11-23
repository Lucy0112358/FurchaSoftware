using Domain.Attributes;
using Domain.Configuration;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authentication;
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
        [HttpGet("users")]
        public ActionResult<ApiResult<List<UserResult>>> GetFilteredUsersWithPagination(
              [FromQuery] int? groupId = null,
              [FromQuery] int? branchId = null,
              [FromQuery] bool? isAdmin = null,
              [FromQuery] string? name = null,
              [FromQuery] int pageNumber = 1,
              [FromQuery] int page = 100)
        {
            var adminId = GetClaimValue("AdminId");
            var users = userService.GetFilteredUsersByPagination(int.Parse(adminId), groupId, branchId, isAdmin, name, pageNumber, page);

            if (users == null || !users.Any())
            {
                return Ok(ApiResult<List<UserResult>>.ErrorResult("No filtered users found for the given criteria."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }

        [Authorize]
        [HttpGet("user-groups")]
        public ActionResult<ApiResult<List<UserGroupResult>>> GetUserGroupsByAdminId([FromQuery] int? branchId = null)
        {
            var adminId = GetClaimValue("AdminId");

            var userGroups = userService.GetUserGroupsForAdminBasedOnRole(
                int.Parse(adminId),
                branchId
            );

            if (userGroups == null || !userGroups.Any())
            {
                return Ok(ApiResult<List<UserGroupResult>>.ErrorResult(
                    "No user groups found for the current admin permissions"));
            }

            return Ok(ApiResult<List<UserGroupResult>>.Success(userGroups));
        }

        [Authorize]
        [HttpGet("search-user")]
        public ActionResult<ApiResult<List<UserResult>>> SearchUsersOfAdmin([FromQuery] string? name)
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

        [HttpPost("edit-user")]
        public async Task<ActionResult<ApiResult<UserResult>>> EditUser([FromBody] UserCreateRequest userCreateRequest)
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
/*        [RequiresPermission("ManageUserGroup")]*/
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

        [HttpPost("delete-users")]
        public IActionResult DeleteUsers([FromBody] DeleteAdminRequest request)
        {
            userService.DeleteUsers(request.Ids);

            return Ok(true);
        }

        [HttpPost("delete-user-groups")]
        public IActionResult DeleteUserGroupss([FromBody] DeleteAdminRequest request)
        {
            userService.DeleteUserGroups(request.Ids);

            return Ok(true);
        }

        [HttpPost("change-state")]
        public IActionResult SetUserState([FromBody] ChangeAdminStateRequest request)
        {
            userService.SetUserState(request.Ids, request.State);

            return Ok();
        }

        [HttpPost("suspend-user-groups")]
        public IActionResult SuspendUserGroups([FromBody] ChangeAdminStateRequest request)
        {
            userService.SuspendUserGroups(request.Ids, request.State);

            return Ok();
        }

        [HttpPatch("change-group")]
        public IActionResult ChangeUserGroup([FromBody] ChangeUsersGroupRequest request)
        {
            userService.ChangeUsersGroup(request.Ids, request.GroupId);

            return Ok();
        }

        [Authorize]
        /*     [RequiresPermission("ManageUserGroup")]*/
        [HttpGet("user-groups/{id}")]
        public ActionResult<ApiResult<GetUserGroupResult>> GetUserGroupById(int id)
        {
            var result = userService.GetUserGroupById(id);

            if (result == null)
                return NotFound(ApiResult<GetUserGroupResult>.ErrorResult("User group not found."));

            return Ok(ApiResult<GetUserGroupResult>.Success(result));
        }


        [HttpGet("{id}")]
        public ActionResult<ApiResult<SingleUserResult>> GetUserById(int id)
        {
            try
            {
                var u = userService.GetUserById(id);

                return Ok(ApiResult<SingleUserResult>.Success(u));
            }
            catch (ArgumentException ex)
            {
                return NotFound((ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest((ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ("An unexpected error occurred."));
            }
        }

        [Authorize]
        [HttpPost("edit-user-group")]
        public ActionResult<ApiResult<UserGroupResult>> EditUserGroup([FromBody] EditUserGroupRequest request)
        {
            if (request == null)
            {
                return BadRequest(ApiResult<UserGroupResult>.ErrorResult("Invalid data."));
            }

            try
            {
                var result = userService.EditUserGroup(request);

                if (result == null)
                {
                    return BadRequest(ApiResult<UserGroupResult>.ErrorResult("Group could not be updated."));
                }

                return Ok(ApiResult<UserGroupResult>.Success(result));
            }
            catch (BaseException ex)
            {
                return BadRequest(ApiResult<UserGroupResult>.ErrorResult(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResult<UserGroupResult>.ErrorResult("Unexpected server error."));
            }
        }

    }
}
