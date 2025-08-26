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
        public ActionResult<ModuleResult> GetModuleById(int id)
        {
            var locker = _lockerService.GetlockersById(id);

            return Ok(ApiResult<ModuleResult>.Success(locker));
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
