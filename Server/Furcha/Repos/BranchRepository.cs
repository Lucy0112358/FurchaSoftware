using Domain.Entities;
using MqttService.Application.Repositories;
using Npgsql;

namespace FurchaAdminApi.Repos
{
    public class BranchRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public BranchRepository(NpgsqlConnection dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Returns a list of Branches by their Ids
        /// </summary>
        public List<Branch> GetBranchesByIds(List<int> branchIds)
        {
            var sql = $@"SELECT * 
                         FROM furcha.""Branch"" 
                         WHERE ""Id"" = ANY(@branchIds)";

            var branches = Query<Branch>(
                sql: sql,
                param: new { branchIds }).ToList();

            return branches;
        }

      
    }
}
