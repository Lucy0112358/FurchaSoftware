using Dapper;
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

        public IEnumerable<LockerWithUsers> GetUserLockersByBranchId(int branchId)
        {
            var query = @"
            SELECT l.""Id"", l.Number, l.""LockerType"", l.""IsActive"", l.""IsOpen"", l.""BranchId"", l.""PasswordHash"", 
                   u.""Name"" AS Name
            FROM furcha.""Locker"" l
            LEFT JOIN furcha.""UserLocker"" ul ON l.""Id"" = ul.""LockerId""
            LEFT JOIN furcha.""User"" u ON ul.""UserId"" = u.Id
            WHERE l.""BranchId"" = @BranchId";

            var connectionString = "Host=localhost;Port=5432;Database=furcha;Username=postgres;Password=7887;";
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var lockers = connection.Query<LockerWithUsers, string, LockerWithUsers>(
                    query,
                    (locker, userName) => {
                        locker.Users ??= new List<string>();
                        if (!string.IsNullOrEmpty(userName))
                        {
                            locker.Users.Add(userName);
                        }
                        return locker;
                    },
                    param: new { BranchId = branchId },
                    splitOn: "Name"
                ).Distinct().ToList();
                return lockers;
            }

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

        /// <summary>
        /// Retrieves all Brain Modules associated with a specific locker group.
        /// </summary>
        /// <param name="groupId">The ID of the locker group.</param>
        /// <returns>A list of Brain Modules associated with the locker group, or null if none are found.</returns>
        public List<BrainModule> GetModulesByGroupId(int groupId)
        {
            var sql = @"
        SELECT bm.*
        FROM furcha.""BrainModule"" bm      
        WHERE ""GroupId"" = @groupid";

            var brainModules = Query<BrainModule>(
                sql: sql,
                param: new { GroupId = groupId }
            ).ToList();

            return brainModules;
        }


        public List<LockerGroup> GetLockerGroupsByBranchId(int branchId)
        {
            var sql = @"
        SELECT lg.*
        FROM furcha.""LockerGroup"" lg
        WHERE lg.""BranchId"" = @BranchId";

            var lockerGroups = Query<LockerGroup>(
                sql: sql,
                param: new { BranchId = branchId }
            ).ToList();

            return lockerGroups;
        }


        internal List<LockerGroup> GetLockerGroupsByUserGroup(int ugId)
        {
            var sql = @"
                    SELECT DISTINCT lg.*
                    FROM furcha.""LockerGroup"" lg
                    INNER JOIN furcha.""Locker"" l ON lg.""Id"" = l.groupid
                    INNER JOIN furcha.""UserGroup_Locker"" ugl ON l.""Id"" = ugl.""LockerId""
                    WHERE ugl.""UserGroupId"" = @UserGroupId";

            var lockerGroups = Query<LockerGroup>(
                sql: sql,
                param: new { UserGroupId = ugId }
            ).ToList();

            return lockerGroups;
        }


        /// <summary>
        /// Retrieves lockers based on the specified locker group
        /// </summary>
        public List<Locker> GetLockersOfGroup(int groupId)
        {
            var sql = @"
        SELECT l.*
        FROM furcha.""Locker"" l
        WHERE l.""groupid"" = @groupid";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { GroupId = groupId }
            ).ToList();

            return lockers;
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
        internal List<LockerWithUsers> GetLockersByCriteria(int branchId, string? lockerType = null, int? lockerGroupId = null, string? status = null, bool? isActive = null)
        {
            var sql = @"
    SELECT 
        l.""Id"",
        l.Number,
        l.""groupid"",
        l.""LockerType"",
        l.""IsActive"",
        l.""IsOpen"",
        l.""BranchId"",
        l.""PasswordHash"",
        u.""Name""
    FROM furcha.""Locker"" l
    LEFT JOIN furcha.""UserLocker"" ul ON l.""Id"" = ul.""LockerId""
    LEFT JOIN furcha.""User"" u ON ul.""UserId"" = u.Id
    WHERE (l.""LockerType"" = @LockerType OR @LockerType IS NULL)
      AND (l.""groupid"" = @LockerGroupId OR @LockerGroupId IS NULL)
      AND (l.""BranchId"" = @BranchId OR @BranchId IS NULL)
      AND (l.""IsOpen"" = @IsActive OR @IsActive IS NULL)
      AND (l.""IsActive"" = @IsActive OR @IsActive IS NULL)";

            var connectionString = "Host=localhost;Port=5432;Database=furcha;Username=postgres;Password=7887;";

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                var lockersDictionary = new Dictionary<int, LockerWithUsers>();

                var lockers = connection.Query<LockerWithUsers, string, LockerWithUsers>(
                    sql,
                    (locker, userName) =>
                    {
                        if (!lockersDictionary.TryGetValue(locker.Id, out var lockerWithUsers))
                        {
                            lockerWithUsers = locker;
                            lockerWithUsers.Users = new List<string>();
                            lockersDictionary[locker.Id] = lockerWithUsers;
                        }

                        if (!string.IsNullOrEmpty(userName))
                        {
                            lockerWithUsers.Users.Add(userName);
                        }

                        return lockerWithUsers;
                    },
                    param: new
                    {
                        LockerType = lockerType,
                        LockerGroupId = lockerGroupId,
                        BranchId = branchId,
                        IsOpen = isActive == true ? 1 : (int?)null,
                        IsActive = isActive == true ? 1 : (int?)null
                    },
                    splitOn: "Name"
                ).Distinct().ToList();

                return lockers;
            }
        }


        /// <summary>
        /// Retrieves all lockers associated with a specific branch.
        /// </summary>
        /// <param name="branchId">The ID of the branch.</param>
        /// <returns>A list of lockers belonging to the specified branch.</returns>
        public List<Locker> GetLockersByBranchId(int branchId)
        {
            var sql = @"
        SELECT l.*
        FROM furcha.""Locker"" l
        INNER JOIN furcha.""LockerGroup"" lg ON l.""groupid"" = lg.""Id""
        WHERE lg.""BranchId"" = @BranchId";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { BranchId = branchId }
            ).ToList();

            return lockers;
        }

        /// <summary>
        /// Retrieves lockers associated with a specific BrainId.
        /// </summary>
        /// <param name="brainId">The ID of the brain module.</param>
        /// <returns>A list of lockers associated with the specified BrainId.</returns>
        public List<Locker> GetLockersByBrainId(int brainId)
        {
            var sql = @"
        SELECT l.*
        FROM furcha.""Locker"" l       
        WHERE l.""BrainId"" = @BrainId";

            var lockers = Query<Locker>(
                sql: sql,
                param: new { BrainId = brainId }
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

        internal Locker CreateLocker(Locker module)
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
