using Domain.Entities;
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
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("companyBranches")]
        [AllowAnonymous]
        public List<Branch> GetAllBranchesOfCompanyByAdminId([FromBody]int adminId)
        {
            var branches = userService.GetAllBranchesOfCompanyByAdminId(adminId);

            return branches;
        }

        [HttpGet("user-groups")]
        public ActionResult<List<UserGroup>> GetUserGroupsByAdminId(int adminId)
        {
            var userGroups = userService.GetUserGroupsByAdminId(adminId);
            return Ok(userGroups);
        }

        [HttpGet("filtered-users")]
        public ActionResult<List<User>> GetFilteredUsers(
           [FromQuery] int? filterByGroupId,
           [FromQuery] int? filterByBranchId,
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10)
        {
            var users = userService.GetFilteredUsersWithPagination(filterByGroupId, filterByBranchId, pageNumber, pageSize);
            return Ok(users);
        }

        [HttpGet("searchUser")]
        public IActionResult SearchUsers([FromQuery] string name)
        {
            var users = userService.SearchUsers(name);
            return Ok(users);
        }

    }
}
