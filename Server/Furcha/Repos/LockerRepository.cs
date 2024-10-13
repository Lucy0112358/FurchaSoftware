using Domain.Configuration;
using Domain.Entities;
using Domain.Repositories;
using Npgsql;

namespace FurchaAdminApi.Repos
{
    public class LockerRepository : BaseRepository
    {
        private readonly NpgsqlConnection _dbConnection;

        public LockerRepository(NpgsqlConnection dbConnection, ISanitizer sanitizer) : base(dbConnection, sanitizer)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Retrieves the lockers that are permitted for a specific user group.
        /// Returns null
        /// </summary>
        /// <param name="ugId">The ID of the user group.</param>
        /// <returns>A list of lockers permitted for the user group, or null if none are found.</returns>
        internal List<Locker> GetPermittedLockersOfUserGroup(int ugId)
        {
            var sql = @"SELECT l.*
                        FROM furcha.""Locker"" l
                        INNER JOIN furcha.""UserGroup_Locker"" ugl ON l.""Id"" = ugl.""LockerId""
                        WHERE ugl.""UserGroupId"" = @UserGroupId";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { UserGroupId = ugId }
            ).ToList();

            return lockers;
        }

        /// <summary>
        /// Retrieves the lockers that a specific user has access to through their user groups.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of lockers that the user has access to, or null if none are found.</returns>
        internal List<Locker> GetUserGroupLockersForUser(int userId)
        {
            var sql = @"SELECT l.*
                        FROM furcha.""Locker"" l
                        INNER JOIN furcha.""UserGroup_Locker"" ugl ON l.""Id"" = ugl.""LockerId""
                        INNER JOIN furcha.""UserGroup"" ug ON ug.""Id"" = ugl.""UserGroupId""
                        INNER JOIN furcha.""User"" u ON u.""UserGroupId"" = ug.""Id""
                        WHERE u.""Id"" = @UserId";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { UserId = userId }
            ).ToList();

            return lockers;
        }

        /// <summary>
        /// Retrieves all lockers assigned to a specific user, including those outside of user groups.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of all lockers assigned to the user, or null if none are found.</returns>
        internal List<Locker> GetAllLockersForUser(int userId)
        {
            var sql = @"
                    SELECT DISTINCT l.*
                    FROM furcha.""Locker"" l
                    LEFT JOIN furcha.""UserLocker"" ul ON l.""Id"" = ul.""LockerId""
                    LEFT JOIN furcha.""User_UserGroup"" uug ON ul.""UserId"" = uug.""UserId""
                    LEFT JOIN furcha.""UserGroup_Locker"" ugl ON l.""Id"" = ugl.""LockerId""
                    LEFT JOIN furcha.""UserGroup"" ug ON ug.""Id"" = ugl.""UserGroupId""
                    WHERE ul.""UserId"" = @UserId OR uug.""UserId"" = @UserId";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { UserId = userId }
            ).ToList();

            return lockers;
        }

        internal List<LockerGroup> GetLockerGroupsByUserGroup(int ugId)
        {
            var sql = @"
                    SELECT DISTINCT lg.*
                    FROM furcha.""LockerGroup"" lg
                    INNER JOIN furcha.""Locker"" l ON lg.""Id"" = l.GroupId
                    INNER JOIN furcha.""UserGroup_Locker"" ugl ON l.""Id"" = ugl.""LockerId""
                    WHERE ugl.""UserGroupId"" = @UserGroupId";

            var lockerGroups = Query<LockerGroup>(
                sql: sql,
                param: new { UserGroupId = ugId }
            ).ToList();

            return lockerGroups;
        }


    }
}
