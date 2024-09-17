using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using MqttService.Application.Repositories;
using MqttService.Infrastructure.Services;

namespace MqttService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private  CardLockerRepository userLockerRepository;
        private MqttClientService mqttClientService;

        public TestController(IDbConnection dbConnection, CardLockerRepository userLockerRepository, MqttClientService mqttClientService)
        {
            _dbConnection = dbConnection;
            this.userLockerRepository = userLockerRepository;
            this.mqttClientService = mqttClientService;
        }

        [HttpGet("lockers")]
        public async Task<IActionResult> GetLockers()
        {           
                 _dbConnection.Open();

                var lockers = await _dbConnection.QueryAsync("SELECT * FROM LockerType");

                return Ok(lockers);           
        }

        [HttpGet("check-user-locker-compatibility")]
        public IActionResult CheckUserLockerCompatibility(int userId, int lockerId)
        {
            using (var dbConnection = new NpgsqlConnection(_dbConnection.ConnectionString))
            {
              //  var isCompatible = userLockerRepository.CanCardOpenLocker(userId, lockerId);

                return Ok();
            }
        }

        [HttpGet]
        public void UniversalTestMethode()
        {
          
        }

    }
}
