using Domain.Configuration;
using Domain.Entities;
using FurchaAdminApi.Models.Request;
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
        private readonly UserService _userService;
        private readonly BranchService _branchService;
        public BranchController(UserService userService, BranchService branchService)
        {
            _userService = userService;
            _branchService = branchService;
        }

        [HttpGet("company-branches")]
        public ActionResult<ApiResult<List<BranchFilterResult>>> GetAllBranchesOfCompanyByAdminId([FromQuery] int adminId)
        {
            var branches = _userService.GetAdminBranches(adminId);

            if (branches == null || !branches.Any())
            {
                return NotFound(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<BranchFilterResult>>.Success(branches));
        }

        [HttpPost("create-branch")]
        public ActionResult<ApiResult<bool>> CreateBranch([FromQuery] int adminId, [FromBody] CreateBranchRequest request)
        {
            var branch = _branchService.CreateBranch(request, adminId);         

            return Ok(ApiResult<bool>.Success(branch));
        }

        [HttpGet("branches")]
        public ActionResult<ApiResult<List<AllBranchResult>>> GetAllBranchesOfCompanyByAdminId([FromQuery] int adminId, [FromQuery] string? name)
        {

            if (name == null )
            {
                var branches = _branchService.GetAllBranches(adminId);

                return Ok(ApiResult<List<AllBranchResult>>.Success(branches));

            }
            else
            {
                var branches = _branchService.GetSearchedBranches(name, adminId);

                return Ok(ApiResult<List<AllBranchResult>>.Success(branches));
            }

        }
    }
}
