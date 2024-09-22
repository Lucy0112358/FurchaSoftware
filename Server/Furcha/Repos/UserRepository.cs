using Domain.Entities;
using FurchaAdminApi.Models.Result;
using MqttService.Application.Repositories;
using Npgsql;
using System.Text.RegularExpressions;

namespace FurchaAdminApi.Repos
{
    public class UserRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public UserRepository(NpgsqlConnection dbConnection) : base(dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public List<User> GetAllActiveUsers()
        {
            return GetAll<User>().ToList();
        }

        /// <summary>
        /// Returns null if no admin with that email exists. It must be handled with ErrorCodeEnum
        /// </summary>
        public Administrator? GetAdminByEmail(string email)
        {
            var sql = $@"SELECT * FROM furcha.""Administrators"" WHERE Email = @email LIMIT 1";
            var admin = Query<Administrator>(
            sql: sql,
            param: new { email });

            return admin.SingleOrDefault();
        }

        public Administrator GetAdminById(int uid)
        {
            return Get<Administrator>(uid);
        }

        /// <summary>
        /// Returns the list of active users in the current branch
        /// </summary>
        private List<User> GetAllUsersOfBranch(int branchId)
        {
            var sql = $@"SELECT T.* 
             FROM furcha.{nameof(User)} T
             INNER JOIN furcha.{nameof(UserBranch)} T1 
             ON T.{nameof(User.Id)} = T1.{nameof(UserBranch.UserId)}
             WHERE T1.{nameof(UserBranch.BranchId)} = @branchId";

            var branchUsers = Query<User>(
                sql: sql,
                param: new { branchId }).ToList();

            return branchUsers;
        }

        internal List<Branch> GetAllBranchesOfCompany(int companyId)
        {
            var sql = $@"SELECT T.* 
             FROM furcha.{nameof(Branch)} T
             INNER JOIN furcha.{nameof(Company)} T1 
             ON T.{nameof(Branch.CompanyId)} = T1.{nameof(Company.Id)}
             WHERE T1.{nameof(Company.Id)} = @companyId";

            var branchUsers = Query<Branch>(
                sql: sql,
                param: new { companyId }).ToList();

            return branchUsers;
        }
        public List<User> SearchUsersByName(string name)
        {
            var sql = $@"SELECT * 
                 FROM furcha.{nameof(User)} 
                 WHERE Name ILIKE @name OR Surname ILIKE @name";

            var users = Query<User>(
                sql: sql,
                param: new { name = $"%{name}%" } // Using wildcards for partial matches
            ).ToList();

            return users;
        }

        /// <summary>
        /// Returns the list of active users in the specified group <br></br>
        /// This is already grouped by branch, as each userGroup is associated with one Branch
        /// </summary>
        private List<User> GetUsersOfUserGroup(int groupId)
        {
            var sql = $@"SELECT T.* 
                  FROM furcha.{nameof(User)} T
                 INNER JOIN furcha.{nameof(User_UserGroup)} T1 
                 ON T.{nameof(User.Id)} = T1.{nameof(User_UserGroup.UserId)}
                 WHERE T1.{nameof(User_UserGroup.UserGroupId)} = @groupId";

            var groupUsers = Query<User>(
                sql: sql,
                param: new { groupId }).ToList();

            return groupUsers;
        }




        /*        public List<User> GetUsersByBranchAndGroup(int groupId, int branchId)
                {
                    var sql = $@"SELECT T.* 
                         FROM furcha.""User"" T
                         INNER JOIN furcha.""User_UserGroup"" T1 
                         ON T.""Id"" = T1.""UserId""
                         INNER JOIN furcha.""UserGroup"" UG
                         ON T1.""UserGroupId"" = UG.""Id""
                         WHERE T1.""UserGroupId"" = @groupId
                         AND UG.""BranchId"" = @branchId";

                    var groupUsers = Query<User>(
                        sql: sql,
                        param: new { groupId, branchId }).ToList();

                    return groupUsers;
                }
        */

        public List<UserGroup> GetUserGroupsByCompanyId(int companyId)
        {
            var sql = $@"SELECT UG.* 
                 FROM furcha.{nameof(UserGroup)} UG
                 INNER JOIN furcha.{nameof(Branch)} B 
                 ON UG.{nameof(UserGroup.BranchId)} = B.{nameof(Branch.Id)}
                 WHERE B.{nameof(Branch.CompanyId)} = @companyId";

            var userGroups = Query<UserGroup>(
                sql: sql,
                param: new { companyId }).ToList();

            return userGroups;
        }

        public List<User> GetFilteredUsersByPagination(int? filterByGroupId = null, int? filterByBranchId = null, int pageNumber = 1, int pageSize = 10)
        {
            List<User> users = new List<User>();
            string sql = "";
            object queryParams = new { };

            int offset = (pageNumber - 1) * pageSize;

            if (filterByGroupId.HasValue)
            {
                sql = $@"SELECT T.* 
                 FROM furcha.{nameof(User)} T
                 INNER JOIN furcha.{nameof(User_UserGroup)} T1 
                 ON T.{nameof(User.Id)} = T1.{nameof(User_UserGroup.UserId)}
                 WHERE T1.{nameof(User_UserGroup.UserGroupId)} = @groupId
                 LIMIT @pageSize OFFSET @offset";

                queryParams = new { groupId = filterByGroupId.Value, pageSize, offset };
            }
            else if (filterByBranchId.HasValue)
            {
                sql = $@"SELECT T.* 
                 FROM furcha.{nameof(User)} T
                 INNER JOIN furcha.{nameof(UserBranch)} T1 
                 ON T.{nameof(User.Id)} = T1.{nameof(UserBranch.UserId)}
                 WHERE T1.{nameof(UserBranch.BranchId)} = @branchId
                 LIMIT @pageSize OFFSET @offset";

                queryParams = new { branchId = filterByBranchId.Value, pageSize, offset };
            }
            else
            {
                users = GetAllActiveUsers()
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return users;
            }

            users = Query<User>(sql: sql, param: queryParams).ToList();

            return users;
        }

    }
}
