using Domain.Configuration;
using Domain.Entities;
using FurchaAdminApi.Models.Request;
using MqttService.Application.Repositories;
using Npgsql;

namespace FurchaAdminApi.Repos
{
    public class UserRepository : BaseRepository
    {

        public UserRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
        {
        }

        /// <summary>
        /// Returns all the users of the company, regardless of a branch
        /// </summary>
        public List<User> GetCompanyUsers(int companyId)
        {
            var sql = $@"SELECT * FROM furcha.{nameof(User)} WHERE ""CompanyId"" = @companyId";
            var usersOfCompany = Query<User>(
            sql: sql,
            param: new { companyId });

            return usersOfCompany.ToList();
        }

        /// <summary>
        /// Returns null if no admin with that email exists. It must be handled with ErrorCodeEnum
        /// </summary>
        public User? GetAdminByEmail(string email)
        {
            var sql = $@"SELECT * FROM furcha.""User"" WHERE Email = @email LIMIT 1";
            var admin = Query<User>(
            sql: sql,
            param: new { email });

            return admin.SingleOrDefault();
        }

        /// <summary>
        /// Returns an administrator by their UserId
        /// </summary>
        public Administrators? GetAdminByUserId(int userId)
        {
            var sql = $@"SELECT * 
                         FROM furcha.""Administrators"" 
                         WHERE ""UserId"" = @userId 
                         LIMIT 1";

            var admin = Query<Administrators>(
                sql: sql,
                param: new { userId });

            return admin.SingleOrDefault();
        }

        public Administrators? GetAdminById(int adminId)
        {
            var sql = $@"
                    SELECT a.*, u.*
                    FROM furcha.""Administrators"" a
                    JOIN furcha.""user"" u ON a.""UserId"" = u.""id""
                    WHERE a.""id"" = @adminId
                    LIMIT 1";

            var admin = Query<Administrators, User>(
                sql: sql,
                map: (admin, user) =>
                {
                    admin.Email = user.Email;
                    admin.Name = user.Name;
                    admin.Surname = user.Surname;
                    admin.Role = user.Role;

                    return admin;
                },
                param: new { adminId });

            return admin.SingleOrDefault();
        }

        public bool IsUserInGroup(int userId, int groupId)
        {
            string sql = $@"SELECT COUNT(1) 
                    FROM furcha.""User_UserGroup"" 
                    WHERE ""UserId"" = @userId 
                    AND ""UserGroupId"" = @groupId";

            return QuerySingleOrDefault<int>(sql, new { userId, groupId }) > 0;
        }


        public bool IsUserInBranch(int userId, int branchId)
        {
            string sql = $@"SELECT COUNT(1) 
                    FROM furcha.{nameof(UserBranch)} 
                    WHERE {nameof(UserBranch.UserId)} = @userId 
                    AND {nameof(UserBranch.BranchId)} = @branchId";

            return QuerySingleOrDefault<int>(sql, new { userId, branchId }) > 0;
        }


        /// <summary>
        /// Returns the list of active users in the current branch
        /// </summary>
        internal List<User> GetAllUsersOfBranch(int branchId)
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

        internal List<Branch> GetAllBranchesOfAdmin(int adminId)
        {
            var sql = $@"SELECT B.* 
                 FROM furcha.""Branch"" B
                 INNER JOIN furcha.""AdminBranch"" AB 
                    ON B.""Id"" = AB.""BranchId""
                 INNER JOIN furcha.""Administrators"" A 
                    ON A.""id"" = AB.""AdministratorId""
                 WHERE A.""id"" = @adminId";

            var branches = Query<Branch>(
                sql: sql,
                param: new { adminId }).ToList();

            return branches;
        }

        public List<Card> GetUserCards(int userId)
        {
            var sql = $@"
            SELECT C.* 
            FROM furcha.""Card"" C
            WHERE C.""UserId"" = @userId";

            return Query<Card>(sql: sql, param: new { userId }).ToList();
        }

        /// <summary>
        /// Returns a list of users that are part of the specified branches
        /// </summary>
        public List<User> GetUsersByBranches(List<int> branchIds)
        {
            var sql = $@"
                SELECT Distinct U.* 
                FROM furcha.User U
                INNER JOIN furcha.UserBranch UB 
                ON U.Id = UB.UserId
                WHERE UB.BranchId = ANY(@branchIds)";

            var users = Query<User>(
                sql: sql,
                param: new { branchIds }).ToList();

            return users;
        }

        public List<UserGroup> GetUserGroupsByBranchIds(List<int> branchIds)
        {
            var sql = @"
                SELECT DISTINCT ug.*
                FROM furcha.""UserGroup"" ug
                INNER JOIN furcha.""UserGroup_Branch"" ugb ON ug.""Id"" = ugb.""UserGroupId""
                WHERE ugb.""BranchId"" = ANY(@branchIds)";

            var userGroups = Query<UserGroup>(
                sql: sql,
                param: new { branchIds }).ToList();

            return userGroups;
        }


        public List<User> SearchUserByAdminId(string name, int adminId, List<int> branchIds)
        {
            var sql = $@"
                    SELECT u.* 
                    FROM furcha.""user"" u
                    INNER JOIN furcha.""userbranch"" ub ON u.""id"" = ub.""userid""
                    WHERE (u.""Name"" ILIKE @name OR u.""Surname"" ILIKE @name)
                    AND ub.""branchid"" = ANY(@branchIds)";

            var users = Query<User>(
                sql: sql,
                param: new
                {
                    name = $"%{name}%",
                    branchIds
                }
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
            var sql = @"
                SELECT UG.* 
                FROM furcha.""UserGroup"" UG
                WHERE UG.""CompanyId"" = @companyId";

            var userGroups = Query<UserGroup>(
                sql: sql,
                param: new { companyId }).ToList();

            return userGroups;
        }

        public List<User> GetFilteredUsersByPagination(int companyId, int? filterByGroupId = null, int? filterByBranchId = null, int pageNumber = 1, int pageSize = 10)
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
                users = GetCompanyUsers(companyId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return users;
            }

            users = Query<User>(sql: sql, param: queryParams).ToList();

            return users;
        }

        /// <summary>
        /// Returns a list of user groups associated with the specified user
        /// </summary>
        public List<UserGroup> GetUserGroupsByUserId(int userId)
        {
            var sql = $@"
                SELECT UG.* 
                FROM furcha.""UserGroup"" UG
                INNER JOIN furcha.""User_UserGroup"" UUG 
                ON UG.""Id"" = UUG.""UserGroupId""
                WHERE UUG.""UserId"" = @userId";

            var userGroups = Query<UserGroup>(
                sql: sql,
                param: new { userId }).ToList();

            return userGroups;
        }

        /// <summary>
        /// Returns a list of branches associated with the specified admin
        /// </summary>
        public List<Branch> GetAdminBranchesByAdminId(int adminId)
        {
            var sql = $@"
                SELECT B.* 
                FROM furcha.""Branch"" B
                INNER JOIN furcha.""AdminBranch"" UB 
                ON B.""Id"" = UB.""BranchId""
                WHERE UB.""AdministratorId"" = @adminId";

            var branches = Query<Branch>(
                sql: sql,
                param: new { adminId }).ToList();

            return branches;
        }

        /// <summary>
        /// Returns a list of branches associated with the specified user
        /// </summary>
        public List<Branch> GetUserBranchesByUserId(int useerId)
        {
            var sql = $@"
                SELECT B.* 
                FROM furcha.""Branch"" B
                INNER JOIN furcha.""userbranch"" UB 
                ON B.""Id"" = UB.""branchid""
                WHERE UB.""userid"" = @useerId";

            var branches = Query<Branch>(
                sql: sql,
                param: new { useerId }).ToList();

            return branches;
        }

        /// <summary>
        /// Inserts a new card into the database.
        /// </summary>
        /// <param name="card">The card to be inserted.</param>
        /// <returns>The inserted card entity.</returns>
        public Card AddCard(UserCreateRequest card)
        {
            var newcard = new Card
            {
                UserId = 10, // Example UserId (assuming there's a user with this ID in your database)
                CardNumber = "1234-5678-9012-3456" // Example Card Number
            };

            return Insert(newcard); // Utilize the Insert method from the base repository
        }
    }
}
