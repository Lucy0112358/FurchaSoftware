using Domain.Configuration;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Services;
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

        public ModulesController(LockerService lockerService)
        {
            _lockerService = lockerService;
        }

      /*  [Authorize]*/
        [HttpGet]
        public ActionResult<ApiResult<List<Models.Result.AllModulesResult>>> Modules(int groupId)
        {
            var adminId = GetClaimValue("AdminId");

            var modules = _lockerService.GetAddedModules(int.Parse(adminId));

            return Ok(ApiResult<List<Models.Result.AllModulesResult>>.Success(modules));
        }

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
            catch (InvalidOperationException ex) // when no lockers
            {
                return BadRequest((ex.Message));
            }
            catch (Exception ex)
            {
                // log exception here
                return StatusCode(500, ("An unexpected error occurred."));
            }
        }


        /*        [Authorize]*/
        [HttpPost("{id}")]
        public ActionResult<bool> Update(int id, [FromBody] UpdateModuleRequest request)
        {
            var locker = _lockerService.UpdateModule(request.LockerType, request.LockerFrom, request.LockerTo, request.LockerGroupId, id);

            return Ok(ApiResult<bool>.Success(locker));
        }

    }
}
