using Domain.Entities;
using Npgsql;

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

        public List<Locker> GetLockersByCardId(int cardId)
        {
            var query = @"
        SELECT l.* 
        FROM ""public"".""Locker"" l
        INNER JOIN ""public"".""LockerCard"" lc
        ON l.""id"" = lc.""lockerId""
        WHERE lc.""cardId"" = @cardId";

            return Query<Locker>(query, new { cardId }).ToList();

        }

        // Add IsActive property later, to indicate subscribed branches
        public List<Branch> GetAllActiveBranches()
        {
            return GetAll<Branch>().ToList();
        }

        public List<BrainModule> GetBrainsByBranchId(int branchId)
        {
            var query = @"
        SELECT * 
        FROM ""public"".""BrainModule""        
        WHERE ""branchId"" = @branchId";

            return Query<BrainModule>(query, new { branchId }).ToList();

        }


    }
}
