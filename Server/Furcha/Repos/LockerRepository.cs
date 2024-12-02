using Domain.Configuration;
using Domain.Entities;
using Domain.Repositories;
using FurchaAdminApi.Models.Result;
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

        /// <summary>
        /// Retrieves lockers based on the specified filter criteria, including full locker details.
        /// </summary>
        /// <param name="lockerType">The type of the locker (optional).</param>
        /// <param name="lockerGroupId">The ID of the locker group (optional).</param>
        /// <param name="branchId">The ID of the branch (optional).</param>
        /// <param name="status">The status of the locker (optional).</param>
        /// <param name="isActive">Indicates if the locker is active (optional).</param>
        /// <param name="lockerStatus">The specific locker status (optional).</param>
        /// <returns>A list of lockers that match the specified criteria, with full details.</returns>
        internal List<Locker> GetLockersByCriteria(int? lockerType = null, int? lockerGroupId = null, int? branchId = null, string? status = null, bool? isActive = null, string? lockerStatus = null)
        {
            var sql = @"
        SELECT l.*, 
               lg.""Id"" AS LockerGroupId, lg.""Name"" AS LockerGroupName,
               b.""Id"" AS BranchId, b.""Name"" AS BranchName
        FROM furcha.""Locker"" l
        LEFT JOIN furcha.""LockerGroup"" lg ON l.""LockerGroupId"" = lg.""Id""
        LEFT JOIN furcha.""Branch"" b ON l.""BranchId"" = b.""Id""
        WHERE (l.""LockerTypeId"" = @LockerType OR @LockerType IS NULL)
          AND (l.""LockerGroupId"" = @LockerGroupId OR @LockerGroupId IS NULL)
          AND (l.""BranchId"" = @BranchId OR @BranchId IS NULL)
          AND (l.""Status"" = @Status OR @Status IS NULL)
          AND (l.""IsActive"" = @IsActive OR @IsActive IS NULL)
          AND (l.""LockerStatus"" = @LockerStatus OR @LockerStatus IS NULL)";

            var lockers = Query<Locker>(
                sql: sql,
                param: new
                {
                    LockerType = lockerType,
                    LockerGroupId = lockerGroupId,
                    BranchId = branchId,
                    Status = status,
                    IsActive = isActive,
                    LockerStatus = lockerStatus
                }
            ).ToList();

            return lockers;
        }

        /// <summary>
        /// Create a locker group and connect it with a branch,
        /// each locker group is associated with one branch physically
        /// </summary>
        internal LockerGroup CreateLockerGroup(LockerGroup group)
        {
            var result = Insert(group);

            return result;
        }

        internal BrainModule CreateModule(BrainModule module)
        {
            var result = Insert(module);

            return result;
        }

#warning after auth get only valid for admin
        public List<LockerGroup> GetLockerGroupsByAdminId(int adminId)
        {
            //string query = @"
            //SELECT lg.Id, lg.Name, lg.Description, lg.State 
            //FROM furcha.""LockerGroup"" lg
            //INNER JOIN furcha.""AdminLockerGroup"" alg ON alg.LockerGroupId = lg.Id
            //WHERE alg.AdminId = @AdminId";
            //INNER JOIN furcha.""AdminLockerGroup"" alg ON alg.LockerGroupId = lg.Id
            //WHERE alg.AdminId = @AdminId";
            string query = @"
            SELECT lg.""Id"", lg.""Name"", lg.""Description"", lg.""BranchId""
            FROM furcha.""LockerGroup"" lg";

            var lockers = Query<LockerGroup>(
    sql: query,
    param: adminId
).ToList();

            return lockers;

        }
    }
}
