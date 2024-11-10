using Microsoft.AspNetCore.Mvc;
using Domain.Entities;
using FurchaAdminApi.Services;
using Domain.Configuration;
using Domain.Extensions;
using FurchaAdminApi.Models.Result;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LockerController : ControllerBase
    {
        private readonly LockerService _lockerService;

        // Inject the LockerService via the constructor
        public LockerController(LockerService lockerService)
        {
            _lockerService = lockerService;
        }

        // GET: api/<LockerController>
        [HttpGet]
        public ActionResult<IEnumerable<Locker>> Get(
            int? lockerType = null,
            int? lockerGroupId = null,
            int? branchId = null,
            string status = null,
            bool? isActive = null,
            string lockerStatus = null)
        {
            var lockers = _lockerService.GetLockersByFilters(
                lockerType,
                lockerGroupId,
                branchId,
                status,
                isActive,
                lockerStatus
            );

            if (lockers == null || !lockers.Any())
            {
                return NotFound();
            }

            return Ok(lockers);
        }

        // GET api/<LockerController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/<LockerController>
        [HttpPost]
        public ActionResult<ApiResult<string>> Post(int branchId, string name)
        {
            if (branchId == null || string.IsNullOrEmpty(name))
            {
                return BadRequest(ApiResult<string>.ErrorResult("Invalid request data."));
            }

            bool isCreated = _lockerService.CreateLockerGroup(branchId, name);

            if (!isCreated)
            {
                return StatusCode(500, ApiResult<string>.ErrorResult("Failed to create LockerGroup."));
            }

            return Ok(ApiResult<string>.Success("LockerGroup created successfully."));
        }

        [HttpGet("admin-lockerGroups")]
        public ActionResult<ApiResult<List<LockerGroup>>> GetUserGroupsByAdminId([FromQuery] int adminId)
        {
            var lockerGroups = _lockerService.GetLockerGroupsByAdminId(adminId);

            if (lockerGroups == null || !lockerGroups.Any())
            {
                return NotFound(ApiResult<List<LockerGroup>>.ErrorResult("No user groups found for the provided admin ID."));
            }

            return Ok(ApiResult<List<LockerGroup>>.Success(lockerGroups));
        }

        // PUT api/<LockerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LockerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
