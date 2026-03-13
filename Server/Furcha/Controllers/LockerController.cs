using Domain.Attributes;
using Domain.Configuration;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using FurchaBLL.Interfaces;
using FurchaDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LockerController : BaseController
    {
        private readonly LockerService _lockerService;
        private readonly IDoorStateService _doorStateService;

        public LockerController(LockerService lockerService, IDoorStateService doorStateService)
        {
            _lockerService = lockerService;
            _doorStateService = doorStateService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<OfficeResult>>> Get(
            int? branchId = null,
            int? lockerType = null,
            int? groupId = null,
            int? status = null,
            string? name = null
          )
        {
            var adminId = GetClaimValue("AdminId");

            var lockers = _lockerService.GetLockersByFilters(
                branchId,
                lockerType,
                groupId,
                status,
                name,
                int.Parse(adminId)
            );

            /* if (lockers == null || !lockers.Any())
             {
                 return NotFound();
             }*/

            return Ok(ApiResult<List<OfficeResult>>.Success(lockers));

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
            try
            {
                bool isCreated = _lockerService.CreateLockerGroup(request.BranchId, request.Name);

                if (!isCreated)
                {
                    return StatusCode(500, ApiResult<string>.ErrorResult("Failed to create LockerGroup."));
                }

                return Ok(ApiResult<string>.Success("LockerGroup created successfully."));
            }
            catch (BaseException e)
            {
                return BadRequest(ApiResult<UserResult>
                    .ErrorResult(e.errorCodeEnum, e.Message));
            }

        }
        /*
                [Authorize]
                [RequiresPermission("AddModule")]*/
        [HttpPost("CreateModule")]
        public ActionResult<ApiResult<bool>> CreateModule([FromBody] ModuleRequest request)
        {
            if (request == null || request.BranchId == 0)
                return BadRequest(ApiResult<bool>.ErrorResult("Invalid request data."));

            var isCreated = _lockerService.CreateModule(request);

            if (!isCreated)
                return StatusCode(500, ApiResult<bool>.ErrorResult("Failed to create module."));

            return Ok(ApiResult<bool>.Success(true));
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
            var modules = _lockerService.GetNewModules(int.Parse(adminId));

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
        /*    [RequiresPermission("ManageLocker")]*/
        [HttpPost("edit-lockers")]
        public ApiResult<EditLockerResult> EditLockers([FromBody] EditLockerRequest request)
        {
            if (request == null || request.LockerIds == null || !request.LockerIds.Any())
            {
                return ApiResult<EditLockerResult>.ErrorResult("Invalid request data.");
            }

            return _lockerService.EditLocker(request.LockerIds, request.Type);
        }

        /*  [Authorize]
          [RequiresPermission("ManageLocker")]*/
        [HttpPost("open-lockers")]
        public ActionResult<ApiResult<string>> OpenLockers([FromBody] List<int> lockerIds)
        {
            var adminId = GetClaimValue("AdminId");

            _lockerService.OpenLockers(lockerIds, int.Parse(adminId));

            return Ok(ApiResult<string>.Success("Lockers opened successfully."));
        }


        [Authorize]
        [RequiresPermission("ManageUsers")]
        [HttpPost("set-user")]
        public ActionResult<ApiResult<string>> SetUser([FromBody] SetUserRequest request)
        {
            _lockerService.SetUser(request.LockerIds, request.UserId);

            return Ok(ApiResult<string>.Success("Lockers updated successfully."));
        }


        public class Model
        {
            public List<int> Ids { get; set; }
            public int State { get; set; }
        }

        [Authorize]
        [RequiresPermission("ManageUsers")]
        [HttpPost("change-mode")]
        public ActionResult<ApiResult<string>> SuspendLockers([FromBody] Model model)
        {
            _lockerService.SuspendLockers(model.Ids, model.State);

            return Ok(ApiResult<string>.Success("Lockers updated successfully."));
        }


        public class EditLockerGroupRequest
        {
            public string Name { get; set; }
        }

        [HttpGet("test-door-status")]
        public async Task<ActionResult> TestDoorStatusBroadcast(int doorId = 1, string status = "Open")
        {
            // Broadcast a test message to all clients
            await _doorStateService.NotifyDoorStatusAsync(doorId, status);

            return Ok(ApiResult<string>.Success($"Broadcasted door {doorId} status '{status}'"));
        }


        [Authorize]
        [HttpPost("editLockerGroup/{id}")]
        public ActionResult Edit(int id, [FromBody] EditLockerGroupRequest request)
        {
            try
            {
                var g = _lockerService.EditGroup(id, request.Name);
                return Ok(ApiResult<bool>.Success(g));
            }
            catch (BaseException e)
            {
                return BadRequest(ApiResult<UserResult>
                    .ErrorResult(e.errorCodeEnum, e.Message));
            }

        }


        [Authorize]
        [HttpGet("getLockerGroup/{id}")]
        public ActionResult<ApiResult<LockerGroupResultDto>> GetLockerGroup(int id)
        {
            var g = _lockerService.GetLockerGroup(id);

            if (g == null)
                return NotFound(ApiResult<LockerGroupResultDto>.ErrorResult("Locker group not found."));

            var dto = new LockerGroupResultDto
            {
                Id = g.Id,
                Name = g.Name,
                // BranchId = g.BranchId
            };

            return Ok(ApiResult<LockerGroupResultDto>.Success(dto));
        }

        [Authorize]
        // [RequiresPermission("ManageLockerGroups")]
        [HttpPost("deleteLockerGroup/{id}")]
        public ActionResult<ApiResult<bool>> DeleteLockerGroup(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResult<bool>.ErrorResult("Invalid locker group id."));

            var isDeleted = _lockerService.DeleteLockerGroup(id);

            if (!isDeleted)
                return NotFound(ApiResult<bool>.ErrorResult("Locker group not found or could not be deleted."));

            return Ok(ApiResult<bool>.Success(true));
        }


    }
}
