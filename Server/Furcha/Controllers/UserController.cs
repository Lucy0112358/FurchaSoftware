using Domain.Entities;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(UserService userService, IHttpContextAccessor httpContextAccessor)
        {
            this.userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("companyBranches")]
        [AllowAnonymous]
        public List<Branch> GetAllBranchesOfCompanyByAdminId([FromBody] int adminId)
        {
            var branches = userService.GetAllBranchesOfCompanyByAdminId(adminId);

            return branches;
        }

        [HttpGet("branch-users")]
        public ActionResult<List<User>> GetAllUsersOfBranch(int id)
        {
 /*           var httpContext = _httpContextAccessor.HttpContext;

            var userClaims = httpContext.User.Claims;
            var nameClaim = userClaims.FirstOrDefault(c => c.Type == "name")?.Value;*/
            var users = userService.GetAllUsersOfBranch(id);

            return Ok(users);
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
