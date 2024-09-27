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
    public class BranchController : ControllerBase
    {
        private readonly UserService userService;
        public BranchController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("company-branches")]
        [AllowAnonymous]
        public ActionResult<ApiResult<List<BranchFilterResult>>> GetAllBranchesOfCompanyByAdminId([FromQuery] int adminId)
        {
            var branches = userService.GetAdminBranches(adminId);

            if (branches == null || !branches.Any())
            {
                return NotFound(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<BranchFilterResult>>.Success(branches));
        }

    }
}
