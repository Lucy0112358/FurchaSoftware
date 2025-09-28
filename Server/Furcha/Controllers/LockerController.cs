using Microsoft.AspNetCore.Mvc;
using FurchaAdminApi.Services;
using Domain.Configuration;
using FurchaAdminApi.Models.Result;
using Microsoft.AspNetCore.Authorization;
using FurchaAdminApi.Models.Request;
using Domain.Attributes;
using FurchaDAL.Models;

namespace FurchaAdminApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LockerController : BaseController
    {
        private readonly LockerService _lockerService;

        // Inject the LockerService via the constructor
        public LockerController(LockerService lockerService)
        {
            _lockerService = lockerService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<OfficeResult>>> Get(
            int? branchId = null,
            string? lockerType = null,
            int? lockerGroupId = null,
            int? isOpen = null,
            string? userName = null
          )
        {
            var adminId = GetClaimValue("AdminId");

            var lockers = _lockerService.GetLockersByFilters(
                branchId,
                lockerType,
                lockerGroupId,
                isOpen,
                userName,
                int.Parse(adminId)
            );

            /* if (lockers == null || !lockers.Any())
             {
                 return NotFound();
             }*/

            return Ok(ApiResult<List<OfficeResult>>.Success(lockers));

        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }


        public class LockerGroupRequest
        {
            public int BranchId { get; set; }
            public string Name { get; set; }
        }

        [Authorize]
        [RequiresPermission("ManageLockerGroups")]
        [HttpPost]
        public ActionResult<ApiResult<string>> Post([FromBody] LockerGroupRequest request)
        {
            if (request == null || request.BranchId == 0 || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest(ApiResult<string>.ErrorResult("Invalid request data."));
            }

            bool isCreated = _lockerService.CreateLockerGroup(request.BranchId, request.Name);

            if (!isCreated)
            {
                return StatusCode(500, ApiResult<string>.ErrorResult("Failed to create LockerGroup."));
            }

            return Ok(ApiResult<string>.Success("LockerGroup created successfully."));
        }
/*
        [Authorize]
        [RequiresPermission("AddModule")]*/
        [HttpPost("CreateModule")]
        public ApiResult<bool> CreateModule([FromBody] ModuleRequest request)
        {
            if (request == null || request.BranchId == 0)
            {
                return ApiResult<bool>.ErrorResult("Invalid request data.");
            }

            bool isCreated = _lockerService.CreateModule(request);

            if (!isCreated)
            {
                return ApiResult<bool>.ErrorResult("Failed to create module.");
            }

            return ApiResult<bool>.Success(true);
        }

        [Authorize]
        [HttpGet("lockers-range")]
        public ActionResult<ApiResult<ModuleLockers>> GetLockersRange(int groupId)
        {
            // add catch
            var modules = _lockerService.GetLockersRange(groupId);

            return Ok(ApiResult<ModuleLockers>.Success(modules));
        }

#warning When auth is done, this methode must return only modules accessible for the logged in admin
/*        [Authorize]
        [HttpGet("GetModules")]
        public ActionResult<ApiResult<List<ModuleResult>>> GetModules()
        {
            var adminId = GetClaimValue("AdminId");
            var modules = _lockerService.GetModules(int.Parse(adminId));

            return Ok(ApiResult<List<ModuleResult>>.Success(modules));
        }*/

        [Authorize]
        [HttpGet("get-new-brains")]
        public ActionResult<ApiResult<List<NewModulesResult>>> GetNewModules([FromQuery] int branchId)
        {
            var adminId = GetClaimValue("AdminId");
            var modules = _lockerService.GetNewModules(branchId);

            return Ok(ApiResult<List<NewModulesResult>>.Success(modules));
        }

        [Authorize]
        [HttpGet("admin-lockerGroups")]
        public ActionResult<ApiResult<List<LockerGroup>>> GetUserGroupsByAdminId()
        {
            var adminId = GetClaimValue("AdminId");

            var lockerGroups = _lockerService.GetLockerGroupsByAdminId(int.Parse(adminId));

            if (lockerGroups == null || !lockerGroups.Any())
            {
                return Ok(ApiResult<List<LockerGroup>>.ErrorResult("No user groups found for the provided admin ID."));
            }

            return Ok(ApiResult<List<LockerGroup>>.Success(lockerGroups));
        }

        // PUT api/<LockerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [Authorize]
        [HttpGet("GetGroupsWithLockers")]
        public ActionResult<ApiResult<List<LockersResult>>> GetGroupsWithLockers([FromQuery] int branchId)
        {
            var groupsWithLockers = _lockerService.GetGroupsWithLockers(branchId);

            /*  if (!groupsWithLockers.Any())
              {
                  return NotFound(ApiResult<List<LockerGroupResult>>.ErrorResult("No groups with lockers found."));
              }*/

            return Ok(ApiResult<List<LockersResult>>.Success(groupsWithLockers));
        }

        [Authorize]
        [RequiresPermission("ManageLocker")]
        [HttpPost("edit-lockers")]
        public ApiResult<EditLockerResult> EditLockers([FromBody] EditLockerRequest request)
        {
            if (request == null || request.LockerIds == null || !request.LockerIds.Any())
            {
                return ApiResult<EditLockerResult>.ErrorResult("Invalid request data.");
            }

            return _lockerService.EditLocker(request.LockerIds, request.Type);
        }

        [Authorize]
        [RequiresPermission("ManageLocker")]
        [HttpDelete("{id}")]

        [Authorize]
        [RequiresPermission("ManageLocker")]
        public void Delete(int id)
        {
        }

      /*  [Authorize]
        [RequiresPermission("ManageLocker")]*/
        [HttpPost("open-lockers")]
        public IActionResult OpenLockers([FromBody] List<int> lockerIds)
        {

            _lockerService.OpenLockers(lockerIds);
            return Ok(ApiResult<string>.Success("Lockers edited successfully."));
        }

        [Authorize]
        [RequiresPermission("ManageUsers")]
        [HttpPost("set-user")]
        public IActionResult SetUser([FromBody] SetUserRequest request)
        {

            _lockerService.SetUser(request.LockerIds, request.UserId);
            return Ok(ApiResult<string>.Success("Lockers edited successfully."));
        }

        [Authorize]
        [RequiresPermission("ManageUsers")]
        [HttpPost("suspend-lockers")]
        public IActionResult SuspendLockers([FromBody] List<int> lockerIds)
        {

            _lockerService.SuspendLockers(lockerIds);
            return Ok(ApiResult<string>.Success("Lockers edited successfully."));
        }


    }
}
