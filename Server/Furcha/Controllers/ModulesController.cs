using Domain.Configuration;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [Authorize]
        [HttpGet]
        public ActionResult<ApiResult<List<AllModulesResult>>> Modules(int groupId)
        {
            var adminId = GetClaimValue("AdminId");

            var modules = _lockerService.GetAllModules(int.Parse(adminId));

            return Ok(ApiResult<List<AllModulesResult>>.Success(modules));
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<ApiResult<List<AllModulesResult>>> GetModuleById(int id)
        {
            var adminId = GetClaimValue("AdminId");

            var modules = _lockerService.GetAllModules(int.Parse(adminId));

            return Ok(ApiResult<List<AllModulesResult>>.Success(modules));
        }

    }
}
