using Domain.Configuration;
using FurchaAdminApi.Models.Result;
using FurchaBLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FurchaAdminApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttTestController : BaseController
    {
        private readonly IMqttService _mqttService;
        public MqttTestController(IMqttService mqttService)
        {
            _mqttService = mqttService;
        }

        [HttpPost("Publish")]
        public async Task<IActionResult> PublishAsync([FromBody] object message, [FromQuery] string topic)
        {
            try
            {
                var result = await _mqttService.PublishAsync(message, topic, false);
                return Ok(new
                {
                    result
                });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
