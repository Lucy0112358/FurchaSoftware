using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using Npgsql;
using MqttService.Application.Repositories;

namespace MqttService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        private  CardLockerRepository userLockerRepository;

        public TestController(IDbConnection dbConnection, CardLockerRepository userLockerRepository)
        {
            _dbConnection = dbConnection;
            this.userLockerRepository = userLockerRepository;
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
                var isCompatible = userLockerRepository.CanCardOpenLocker(userId, lockerId);

                return Ok(isCompatible);
            }
        }

        [HttpGet]
        public void UniversalTestMethode()
        {
            var isCompatible = userLockerRepository.CanCardOpenLocker(1, 1);
        }

    }
}
