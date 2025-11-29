using Domain.Attributes;
using Domain.Configuration;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : BaseController
    {
        private readonly UserService _userService;
        private readonly BranchService _branchService;
        public BranchController(UserService userService, BranchService branchService)
        {
            _userService = userService;
            _branchService = branchService;
        }

/*        [Authorize]*/
        [HttpGet("company-branches")]
        public ActionResult<ApiResult<List<BranchFilterResult>>> GetAllBranchesOfCompanyByAdminId()
        {
            var adminId = GetClaimValue("AdminId");
            var branches = _userService.GetAdminBranches(int.Parse(adminId));

            if (branches == null || !branches.Any())
            {
                return Ok(ApiResult<List<BranchFilterResult>>.ErrorResult("No branches found for the provided admin ID."));
            }

            return Ok(ApiResult<List<BranchFilterResult>>.Success(branches));
        }

        [Authorize]
        [RequiresPermission("CreateBranch")]
        [HttpPost("create-branch")]
        public ActionResult<ApiResult<bool>> CreateBranch([FromBody] CreateBranchRequest request)
        {
            var adminId = GetClaimValue("AdminId");
            var branch = _branchService.CreateBranch(request, int.Parse(adminId));         

            return Ok(ApiResult<bool>.Success(branch));
        }

        [Authorize]
        [HttpGet("branches")]
        public ActionResult<ApiResult<List<AllBranchResult>>> GetAllBranchesOfCompanyByAdminId([FromQuery] string? name)
        {
            var adminId = int.Parse(GetClaimValue("AdminId"));

            var branches = _branchService.GetBranches(adminId, name);

            return Ok(ApiResult<List<AllBranchResult>>.Success(branches));
        }


        //  [Authorize]
        //   [RequiresPermission("CreateBranch")]
        [HttpPost("edit-branch")]
        public ActionResult<ApiResult<bool>> EditBranch([FromQuery] int id, [FromBody] CreateBranchRequest request)
        {
           // var AdminId = GetClaimValue("AdminId");
            var branch = _branchService.EditBranch(request,  id);

            return Ok(ApiResult<bool>.Success(branch));
        }

      //  [Authorize]
      //  [RequiresPermission("DeleteBranch")]
        [HttpPost("delete-branch/{id}")]
        public ActionResult<ApiResult<bool>> DeleteBranch(int id)
        {
         //   var AdminId = GetClaimValue("AdminId");
            _branchService.DeleteBranch(id);

      

            return Ok(ApiResult<bool>.Success(true));
        }
    }
}
