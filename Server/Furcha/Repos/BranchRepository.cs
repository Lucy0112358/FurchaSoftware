using Domain.Configuration;
using Domain.Entities;
using Domain.Repositories;
using Npgsql;

namespace FurchaAdminApi.Repos
{
    public class BranchRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public BranchRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
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

        public Company GetCompanyById(int Id)
        {
            return GetSingle<Company>(Id);
        }

        /// <summary>
        /// Gets the branches, which are accessible to the logged in administrator
        /// </summary>
        public List<Branch> GetBranchesByAdminId(int adminId)
        {
            var sql = @"
        SELECT b.*
        FROM furcha.""Branch"" b
        INNER JOIN furcha.""AdminBranch"" ab ON b.""Id"" = ab.""BranchId""
        WHERE ab.""AdministratorId"" = @AdminId";

            var branches = Query<Branch>(
                sql: sql,
                param: new { AdminId = adminId }
            ).ToList();

            return branches;
        }


        public List<Branch> GetBranchesOfUserGroup(int userGroupId)
        {
            var sql = @"
                    SELECT b.*
                    FROM furcha.""Branch"" b
                    INNER JOIN furcha.""UserGroup_Branch"" ugb ON b.""Id"" = ugb.""BranchId""
                    WHERE ugb.""UserGroupId"" = @UserGroupId";

            var branches = Query<Branch>(
                sql: sql,
                param: new { UserGroupId = userGroupId }
            ).ToList();

            return branches;
        }


    }
}
