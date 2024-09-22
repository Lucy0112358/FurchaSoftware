using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Services;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService = FurchaAdminApi.Services.AuthenticationService;

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
        private readonly AuthenticationService authenticationService;

        public AdminTestController(ILogger<AdminTestController> logger, UserService userService, AuthenticationService authenticationService)
        {
            _logger = logger;
            _userService = userService;
            this.authenticationService = authenticationService;

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



    }
}
