using Domain.Configuration;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Services;
using FurchaBLL.Interfaces;
using FurchaBLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : BaseController
    {
        private readonly LockerService _lockerService;
        private readonly IDeviceStatusNotifier _deviceStatusNotifier;

        public ModulesController(LockerService lockerService, IDeviceStatusNotifier deviceStatusNotifier)
        {
            _lockerService = lockerService;
            _deviceStatusNotifier = deviceStatusNotifier;
        }

        [HttpGet("send-brain-status")]
        public async Task<ActionResult> TestDeviceStatus([FromQuery] int companyId, int brainId, string status = "Online") // status could be Offline, Online
        {
            await _deviceStatusNotifier.NotifyBrainStatusAsync(companyId, brainId, status);

            return Ok(ApiResult<string>.Success($"Sent {companyId} {brainId} status '{status}'"));
        }


        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<Models.Result.AllModulesResult>>> Modules(int groupId)
        {
            var adminId = GetClaimValue("AdminId");

            var modules = _lockerService.GetAddedModules(int.Parse(adminId));

            return Ok(ApiResult<List<Models.Result.AllModulesResult>>.Success(modules));
        }

        [Authorize]
        [HttpGet("status-count")]
        public ActionResult<ApiResult<Models.Result.BrainsStatusCountResult>> GetBrainsStatusCount()
        {
            var adminId = GetClaimValue("AdminId");

            var result = _lockerService.GetBrainsOnlineOfflineCount(int.Parse(adminId));

            return Ok(ApiResult<Models.Result.BrainsStatusCountResult>.Success(result));
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ApiResult<ModuleResult>> GetModuleById(int id)
        {
            try
            {
                var module = _lockerService.GetlockersById(id);

                return Ok(ApiResult<ModuleResult>.Success(module));
            }
            catch (ArgumentException ex) // when module not found
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
        [HttpPost("{id}")]
        public ActionResult<ApiResult<bool>> Update(int id, [FromBody] UpdateModuleRequest request)
        {
            try
            {
                var result = _lockerService.UpdateModule(
                    request.LockerType,
                    request.LockerFrom,
                    request.LockerTo,
                    request.LockerGroupId,
                    id
                );

                return Ok(ApiResult<bool>.Success(result));
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                // _logger.LogError(ex, "Error updating locker module");

                return BadRequest(ApiResult<bool>.ErrorResult(ex.Message));
            }
        }


        [Authorize]
        [HttpPost("delete/{id}")]
        public ActionResult<ApiResult<bool>> Delete(int id)
        {
            try
            {
                var deleted = _lockerService.DeleteModule(id);

                if (!deleted)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Module not found."));
                }

                return Ok(ApiResult<bool>.Success(true));
            }
            catch (Exception ex)
            {
                // log exception here
                return StatusCode(500, ApiResult<bool>.ErrorResult("An unexpected error occurred while deleting the module."));
            }
        }

        [Authorize]
        [HttpPost("editModule/{id}")]
        public ActionResult<ApiResult<bool>> Edit(int id, [FromBody] EditModuleDto req)
        {
            var locker = _lockerService.EditModule(id, req.BranchId);
            return Ok(ApiResult<bool>.Success(locker));
        }

        [Authorize]
        [HttpGet("getModule/{id}")]
        public ActionResult<ApiResult<FurchaBLL.Models.EditModuleResult>> GetModule(int id)
        {
            var m = _lockerService.GetModuleById(id);
            return Ok(ApiResult<FurchaBLL.Models.EditModuleResult>.Success(m));
        }

        [Authorize]
        [HttpPost("delete-brain-id/{id}")]
        public ActionResult<ApiResult<bool>> DeleteModuleById(int id)
        {
            try
            {
                var result = _lockerService.DeleteModulePermanently(id);
                return Ok(ApiResult<bool>.Success(result));
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return BadRequest(ApiResult<bool>.ErrorResult(ex.Message));
            }
        }

    }
}
