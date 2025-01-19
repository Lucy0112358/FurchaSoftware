using Domain.Configuration;
using Domain.Entities;
using Domain.Repositories;
using Npgsql;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;

namespace FurchaAdminApi.Repos
{
    public class BranchRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;
        private readonly UserRepository _userRepository;

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

        internal Branch CreateBranch(Branch branch)
        {
            var result = Insert(branch);

            return result;
        }

        internal BranchAddress CreateBranchAddress(BranchAddress address)
        {
            var result = Insert(address);

            return result;
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

#warning access only those linked to admin
        public List<Branch> GetAllBranches(int companyId)
        {
            return GetAll<Branch>(where: $"\"CompanyId\" = {companyId}").ToList();
        }

#warning add auth adminId
        public List<Branch> SearchBranchByAdminId(string name, int adminId)
        {
            var sql = $@"
                    SELECT b.* 
                    FROM furcha.""Branch"" b
                    WHERE (b.""Name"" ILIKE @name)";

            var branches = Query<Branch>(
                sql: sql,
                param: new
                {
                    name = $"%{name}%",
                }
            ).ToList();

            return branches;
        }

        public BranchAddress GetBranchAddressById(int id)
        {
            var res = GetSingle<BranchAddress>(where: $"\"Id\" = @id", whereParam: new { id });

            return res;
        }
        public List<string> GetLockerTypesByBranch(int branchId)
        {
            var sql = @"
    SELECT 
        ARRAY_AGG(DISTINCT ""LockerType"") AS ""LockerTypes""
    FROM 
        furcha.""Locker""
    WHERE 
        ""BranchId"" = @BranchId";

            var result = Query<string[]>(
                sql: sql,
                param: new { BranchId = branchId }
            ).FirstOrDefault();

            return result?.ToList() ?? new List<string>();
        }


    }
}
