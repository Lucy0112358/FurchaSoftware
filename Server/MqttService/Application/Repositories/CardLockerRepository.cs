using Domain.Configuration;
using Domain.Entities;
using Npgsql;

namespace MqttService.Application.Repositories
{
    public class CardLockerRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public CardLockerRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
        {
            _dbConnection = dbConnection;
        }

        public List<Locker> GetLockersByCardId(int cardId)
        {
            var query = @"
        SELECT l.* 
        FROM ""furcha"".""Locker"" l
        INNER JOIN ""furcha"".""UserLocker"" ul
        ON l.""Id"" = ul.""LockerId""
        INNER JOIN ""furcha"".""UserCard"" uc
        ON ul.""UserId"" = uc.""UserId""
        WHERE uc.""CardId"" = @cardId";

            return Query<Locker>(query, new { cardId }).ToList();
        }


        public List<Branch> GetAllActiveBranches()
        {
            return GetAll<Branch>().ToList();
        }

        public List<BrainModule> GetBrainsByBranchId(int branchId)
        {
            var query = @"
        SELECT * 
        FROM ""furcha"".""BrainModule""        
        WHERE ""BranchId"" = @branchId";

            return Query<BrainModule>(query, new { branchId }).ToList();

        }


    }
}
