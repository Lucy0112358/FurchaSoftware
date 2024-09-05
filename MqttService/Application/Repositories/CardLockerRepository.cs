using Npgsql;
using System.Linq;

namespace MqttService.Application.Repositories
{
    public class CardLockerRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public CardLockerRepository(NpgsqlConnection dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public bool CanCardOpenLocker(int? cardId, int? lockerId)
        {
            var query = @"SELECT ""lockerId""
                  FROM ""public"".""LockerCard""
                  WHERE ""cardId"" = @cardId";

            var lockerIds = Query<int>(query, new { cardId }).ToList();

            return lockerIds.Contains((int)lockerId);
        }
    }
}
