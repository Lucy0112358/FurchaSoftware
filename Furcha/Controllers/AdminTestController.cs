using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Furcha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminTestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly UserService _userService;
        private readonly ILogger<AdminTestController> _logger;

        public AdminTestController(ILogger<AdminTestController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetFilteredUsers")]
        public IActionResult GetFilteredUsers(
            [FromQuery] int? groupId = null,
            [FromQuery] int? branchId = null,
            [FromQuery] int pageNumber = 1,  
            [FromQuery] int pageSize = 10)   
        {
                var users = _userService.GetFilteredUsersWithPagination(groupId, branchId, pageNumber, pageSize);

                return Ok(users);
        }

    }
}
