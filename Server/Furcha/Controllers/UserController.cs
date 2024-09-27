using Domain.Configuration;
using Domain.Entities;
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

            var userClaims = httpContext.User.Claims;
            var nameClaim = userClaims.FirstOrDefault(c => c.Type == "name")?.Value;
            var users = userService.GetUsersForAdminRole(adminId);

            if (users == null || !users.Any())
            {
                return NotFound(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<UserResult>>.Success(users));
        }


        [HttpGet("filtered-users")]
        public ActionResult<List<User>> GetFilteredUsersWithPagination(
                 [FromQuery] int companyId,
                 [FromQuery] int? filterByGroupId,
                 [FromQuery] int? filterByBranchId,
                 [FromQuery] int pageNumber = 1,
                 [FromQuery] int pageSize = 10)
        {
            var users = userService.GetFilteredUsersWithPagination(companyId, filterByGroupId, filterByBranchId, pageNumber, pageSize);

            return Ok(users);
        }


        [HttpGet("user-groups")]
        public ActionResult<List<UserGroup>> GetUserGroupsByAdminId(int adminId)
        {
            var userGroups = userService.GetUserGroupsByAdminId(adminId);
            return Ok(userGroups);
        }



        [HttpGet("searchUser")]
        public IActionResult SearchUsers([FromQuery] string name)
        {
            var users = userService.SearchUsers(name);
            return Ok(users);
        }

    }
}
