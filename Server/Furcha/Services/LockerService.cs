using Domain.Configuration;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Models;
using FurchaBLL.MqttModels.Subscribe;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Locker = FurchaDAL.Models.Locker;
using LockerGroup = FurchaDAL.Models.LockerGroup;

namespace FurchaAdminApi.Services
{
    public class LockerService
    {
        private readonly LockerRepository _lockerRepository;
        private readonly BranchRepository _branchRepository;
        private readonly UserRepository _userRepository;
        private readonly furchaContext Db;
        private readonly MqttService _mqttService;

        public LockerService(LockerRepository lockerRepository, BranchRepository branchRepository, UserRepository userRepository, MqttService mqttService, furchaContext db)
        {
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _mqttService = mqttService;
            Db = db;
        }
        /// <summary>
        /// Each user in the mLockers should have access to same editingLockers in the lockerGroup. <br></br>
        /// Each locker is in only one lockerGroup.
        /// </summary>
        /// <param name="ugId">The ID of the user mLockers.</param>
        /// <returns>A list of permitted editingLockers for the user mLockers.</returns>
        public List<Locker> GetPermittedLockersOfUserGroup(int ugId)
        {
            return Db.UserGroupLockers
                .Where(ugl => ugl.UserGroupId == ugId)
                .Join(Db.Lockers,
                      ugl => ugl.LockerId,
                      locker => locker.Id,
                      (ugl, locker) => locker)
                .Include(l => l.Brain)     // optional
                .ToList();
        }
        public List<OfficeResult> GetLockersByFilters(
            int? branchId,
            int? lockerType,
            int? lockerGroupId,
            int? status = null,
            string? userName = null,
            int? adminId = null)
        {
            // 1. Get branches of admin
            var adminBranches = Db.Branches
                .Include(b => b.AdminBranches)
                .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId) &&
                      (branchId == null || b.Id == branchId))
                .ToList();

            var results = new List<OfficeResult>();

            foreach (var branch in adminBranches)
            {
                // 2. Load lockers for this branch, ordered by Number
                var lockers = Db.Lockers
                    .Include(l => l.Users)
                    .Include(l => l.Brain)
                    .Include(l => l.LockerTypeNavigation)
                    .Where(l =>
                        l.IsDeleted == false &&
                        l.Brain.BranchId == branch.Id &&
                        (lockerType == null || l.LockerType == lockerType) &&
                        (lockerGroupId == null || l.Brain.GroupId == lockerGroupId) &&
                        (userName == null || l.Users.Any(u => u.Name != null && u.Name.Contains(userName))) &&
                        (status == null || l.LockerStatus == status)
                    )
                    .OrderBy(l => l.Number)                    // ← sort by locker number
                    .Select(l => new LockerWithUsers
                    {
                        Id = l.Id,
                        number = (long)(l.Number ?? 0),
                        groupid = l.Brain.GroupId,
                        BrainId = l.Brain.Id,
                        LockerType = l.LockerTypeNavigation,
                        IsActive = l.IsActive ?? 0,
                        Status = l.LockerStatus ?? 0,
                        IsOpen = l.IsOpen == 1 ? 1 : 0,
                        BranchId = l.Brain.BranchId ?? 0,
                        PasswordHash = l.PasswordHash,
                        Users = l.Users
                            .Where(u => u.Name != null)
                            .Select(u => u.Name)
                            .ToList()
                    })
                    .ToList();

                // 3. Group lockers by LockerGroups
                var lockerGroups = Db.LockerGroups
                    .Where(lg => lg.BranchId == branch.Id)
                    .ToList();

                var lockerResults = new List<LockersResult>();

                // Assigned groups
                foreach (var group in lockerGroups)
                {
                    var groupLockers = lockers
                        .Where(l => l.groupid == group.Id)
                        .OrderBy(l => l.number)              // ← keep order inside group
                        .ToList();

                    lockerResults.Add(new LockersResult
                    {
                        Id = group.Id,
                        GroupName = group.Name,
                        GroupLockers = groupLockers
                    });
                }

                // 4. UNASSIGNED LOCKERS: group by MODULE (BrainId)
                var unassigned = lockers.Where(l => l.groupid == null || l.groupid == 0);

                var unassignedByModule = unassigned
                    .GroupBy(l => l.BrainId)
                    .ToList();

                int index = 1;

                foreach (var moduleGroup in unassignedByModule)
                {
                    lockerResults.Add(new LockersResult
                    {
                        Id = 0,
                        GroupName = $"Unassigned{index}",
                        GroupLockers = moduleGroup
                            .OrderBy(l => l.number)          // ← ordered here too
                            .ToList()
                    });

                    index++;
                }

                results.Add(new OfficeResult
                {
                    OfficeName = branch.Name,
                    Lockers = lockerResults
                });
            }

            return results;
        }

        public List<LockersResult> GetGroupsWithLockers(int branchId)
        {
            var lockerGroups = Db.LockerGroups
                .Where(lg => lg.BranchId == branchId)
                .Distinct()
                .ToList();

            var lockers = Db.Lockers
                 .Include(x => x.Brain)
                 .Include(l => l.LockerTypeNavigation)
                 .Where(l => l.Brain.BranchId == branchId)
                 .Select(l => new LockerWithUsers
                 {
                     Id = l.Id,
                     groupid = l.Brain.GroupId,
                     number = (long)(l.Number ?? 0),
                     LockerType = l.LockerTypeNavigation,
                     IsActive = (int)l.IsActive,
                     Status = (int)l.LockerStatus,
                     IsOpen = l.IsOpen == 1 ? 1 : 0,
                     BranchId = l.Brain.BranchId ?? 0,
                     PasswordHash = l.PasswordHash,
                     Users = l.Users.Select(u => u.Name).Distinct().ToList()
                 })
                 .ToList();

            var results = lockerGroups
                .Select(group => new LockersResult
                {
                    GroupName = group.Name,
                    GroupLockers = lockers
                        .Where(locker => locker.groupid == group.Id)
                        .OrderBy(locker => locker.number)       // ← order inside group
                        .ToList()
                })
                .Where(result => result.GroupLockers.Any())
                .ToList();

            return results;
        }

        public List<NewModulesResult> GetNewModules(int adminId)
        {
            var accountId = Db.Administrators
                .Where(a => a.Id == adminId)
                .Select(a => a.CompanyId)
                .FirstOrDefault(); // _userRepository.GetCompanyIdByAdminId(AdminId);

            // var lockerGroups = _lockerRepository.GetBrainModulesByBranchAndStatus(4, 1);
            var lockerGroups = Db.BrainModules
              .Where(bm => bm.Status == 1)
              .ToList();

            var results = lockerGroups.Select(l => new NewModulesResult
            {
                Info = l.Description,
                Id = l.Id,
                MacAddress = l.MacAddress,
            }).ToList();

            return results;
        }

        public ModuleLockers GetLockersRange(int groupId)
        {
            var lockersWithNumber = Db.Lockers.Include(x => x.Brain)
        .Where(l => l.Brain.GroupId == groupId).ToList() //_lockerRepository.GetLockersOfGroup(oldGroupId)
                 .OrderBy(x => x.Number).Select(x => x.Number).ToList();

            return new ModuleLockers
            {
                FirstLocker = (int)lockersWithNumber.FirstOrDefault(),
                LastLocker = (int)lockersWithNumber.LastOrDefault()
            };
        }

        /// <summary>
        /// Retrieves editingLockers based on specified filtering criteria.
        /// </summary>
        public List<Models.Result.ModuleResult> GetModules(int adminId)
        {
#warning auth
            var adminBranches = Db.Branches
        .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId))
        .ToList(); //_branchRepository.GetBranchesByAdminId(8);

            var result = new List<Models.Result.ModuleResult>();


            foreach (var branch in adminBranches)
            {
                var branchLockerGroups = Db.LockerGroups
        .Where(lg => lg.BranchId == branch.Id)
        .ToList();// _lockerRepository.GetLockerGroupsByBranchId(branch.Id);

                var moduleInfos = new List<ModuleInfo>();

                foreach (var group in branchLockerGroups)
                {
                    var groupModules = Db.BrainModules
        .Where(bm => bm.GroupId == group.Id && bm.Status == 2)
        .ToList(); //_lockerRepository.GetModulesByGroupId(mLockers.Id);

                    if (groupModules.Any())
                    {
                        var moduleLockers = new List<ModuleLockers>();

                        foreach (var module in groupModules)
                        {
                            var lockers = Db.Lockers
                                .Where(l => l.BrainId == module.Id && !l.IsDeleted)
                                .OrderBy(l => l.Number)
                                .ToList();

                            if (!lockers.Any()) continue;

                            moduleLockers.Add(new ModuleLockers
                            {
                                FirstLocker = (int)(lockers.First().Number ?? 0),
                                LastLocker = (int)(lockers.Last().Number ?? 0)
                            });
                        }


                        moduleInfos.Add(new ModuleInfo
                        {
                            GroupName = group.Name,
                            GroupModules = moduleLockers
                        });
                    }
                }

                if (moduleInfos.Any())
                {
                    result.Add(new Models.Result.ModuleResult
                    {
                        OfficeName = branch.Name,
                        Modules = moduleInfos
                    });
                }
            }

            return result;
        }

        internal bool CreateModule(ModuleRequest request)
        {
            // 1. Find module with its lockers
            var module = Db.BrainModules
                .Include(m => m.Lockers)
                .FirstOrDefault(m => m.Id == request.BrainId);

            if (module == null)
                throw new ArgumentException($"Module with Id {request.BrainId} not found");

            // 2. Attach module to branch and mark as "added"
            module.BranchId = request.BranchId;
            module.Status = 2;          // already added/active
            module.GroupId = null;      // not in any group yet (Unassigned)

            // 3. Restore lockers:
            //    - make them visible (IsDeleted = false)
            //    - give them numbers 1..N inside module
            //    - default type = Unspecified (Id = 1)
            var moduleLockers = module.Lockers
                .OrderBy(l => l.Id)
                .ToList();

            int number = 1;
            foreach (var locker in moduleLockers)
            {
                locker.IsDeleted = false;

                // always renumber 1..N for a new module without group
                locker.Number = number++;

                // default type: "Unspecified"
                locker.LockerType = 1; // LockerType.Id = 1 → Unspecified
            }

            Db.SaveChanges();
            return true;
        }


        internal bool CreateLockerGroup(int branchId, string name)
        {
            var result = new LockerGroup()
            {
                BranchId = branchId,
                Name = name
            };

            Db.LockerGroups.Add(result);
            Db.SaveChanges();

            // _lockerRepository.CreateLockerGroup(result);

            return true;
        }

        /// <summary>
        /// Retrieves all locker groups associated with a specific admin ID.
        /// </summary>
        /// <param name="adminId">The ID of the admin.</param>
        /// <returns>A list of locker groups associated with the specified admin.</returns>
        public List<LockerGroup> GetLockerGroupsByAdminId(int adminId)
        {
            var lockerGroups = (
                from ab in Db.AdminBranches
                join lg in Db.LockerGroups on ab.BranchId equals lg.BranchId
                where ab.AdministratorId == adminId
                select lg
            ).ToList();

            //_lockerRepository.GetLockerGroupsByAdminId(AdminId);
            return lockerGroups;
        }

        public ApiResult<EditLockerResult> EditLocker(List<int> lockerIds, int type)
        {
            var result = new EditLockerResult();

            var editingLockers = Db.Lockers
                .Include(l => l.Brain)
                .Where(l => lockerIds.Contains(l.Id))
                .ToList();

            if (!editingLockers.Any())
                return ApiResult<EditLockerResult>.ErrorResult("No lockers selected.");

            var groupedLockers = editingLockers
                .GroupBy(l => l.Brain.GroupId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // --- resolve locker type from input (id OR name OR code) ---
            type = type?.Trim() ?? string.Empty;
            int parsedId;
            bool isId = int.TryParse(type, out parsedId);
            string lowered = type.ToLower();

            var dbType = Db.LockerTypes.FirstOrDefault(t =>
                    (isId && t.Id == parsedId) ||
                    t.Name.ToLower() == lowered ||
                    t.Type.ToLower() == lowered);

            if (dbType == null)
                return ApiResult<EditLockerResult>.ErrorResult("Locker type not found.");

            int tId = dbType.Id;

            const int unspecifiedId = 1;
            const int commonId = 3;
            const int personalId = 4;

            foreach (var lockerGroup in groupedLockers)
            {
                using var transaction = Db.Database.BeginTransaction();
                try
                {
                    var lockersInSelection = lockerGroup.Value;
                    var groupId = lockerGroup.Key;

                    // all lockers in this GROUP (not only selected)
                    var allGroupLockers = Db.Lockers
                        .Include(l => l.Brain)
                        .Where(l => l.Brain.GroupId == groupId && !l.IsDeleted)
                        .ToList();

                    var nonSelected = allGroupLockers
                        .Where(l => !lockerIds.Contains(l.Id))
                        .ToList();

                    // collect resulting types, ignoring Unspecified
                    var finalTypes = nonSelected
                        .Select(l => l.LockerType)
                        .Where(t => t != unspecifiedId)
                        .Distinct()
                        .ToList();

                    if (tId != unspecifiedId && !finalTypes.Contains(tId))
                        finalTypes.Add(tId);

                    if (finalTypes.Any())
                    {
                        bool allSame = finalTypes.Count == 1;
                        bool onlyCommonAndPersonal =
                            finalTypes.All(t => t == commonId || t == personalId);

                        if (!allSame && !onlyCommonAndPersonal)
                        {
                            throw new BaseException(
                                ErrorCodeEnum.GenericErrorRetry,
                                "Only 'Common' and 'Personal' locker types can be mixed in one group. " +
                                "Other types must be used alone in a group.");
                        }
                    }

                    // compatible → apply new type to selected lockers
                    foreach (var locker in lockersInSelection)
                    {
                        locker.LockerType = tId;
                    }

                    Db.SaveChanges();
                    transaction.Commit();

                    if (groupId.HasValue)
                        result.SuccessfulGroups.Add(groupId.Value);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    var gid = lockerGroup.Key ?? 0;
                    result.FailedGroups.Add((gid, ex.Message));
                }
            }

            return ApiResult<EditLockerResult>.Success(result);
        }


        public void SuspendLockers(List<int> lockerIds)
        {
            foreach (var id in lockerIds)
            {
                var locker = Db.Lockers.FirstOrDefault(l => l.Id == id);
                if (locker == null) continue;

                locker.IsActive = locker.IsActive;
            }

            Db.SaveChanges();
        }

        public async void OpenLockers(List<int> lockerIds, int adminId)
        {
            var brainUid = string.Empty;
            var lockers = Db.Lockers.Include(l => l.Brain).Where(l => lockerIds.Contains(l.Id)).ToList();
            var lockersByBrain = lockers.GroupBy(l => l.Brain.BrainUid);
            var companyUid = "469d54ff-f67d-4aa2-91df-3349a960233a";// Db.Administrators.Include(a => a.Company).Select(a => a.Company.AccountUid).FirstOrDefault();

            foreach (var brainLockers in lockersByBrain)
            {
                var ids = brainLockers.Select(l => l.ExternalId).ToList();

                var data = new LockerData
                {
                    Count = ids.Count,
                    Type = "Locker",
                    Ids = ids
                };

                var mqttRequest = new MqttBaseRequest<LockerData>
                {
                    Command = (int)CommandTypes.OpenLockersFromAdmin,
                    ReceivedDate = DateTime.UtcNow,
                    Data = data
                };

                await _mqttService.PublishToMqtt(mqttRequest, $"controller/{companyUid}/{brainLockers.Key}");
            }
        }

        public void SetUser(List<int> lockerIds, int userId)
        {
            // make isactive to 0, or 1
            AssignLockersToUser(lockerIds, userId);
        }

        private void AssignLockersToUser(List<int> lockerIds, int userId)
        {
            var user = Db.Users.Include(u => u.Lockers).FirstOrDefault(u => u.Id == userId);
            if (user == null) throw new Exception("User not found");

            var lockers = Db.Lockers.Where(l => lockerIds.Contains(l.Id)).ToList();

            foreach (var locker in lockers)
            {
                if (!user.Lockers.Contains(locker))
                    user.Lockers.Add(locker);
            }

            Db.SaveChanges();
        }

        public List<AllModulesResult> GetAddedModules(int adminId)
        {
            var result = Db.Branches
                .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId))
                .Select(b => new AllModulesResult
                {
                    BranchName = b.Name,
                    Modules = b.BrainModules
                        .Where(x => x.Status == 2)
                        .Select(x => new BranchModules
                        {
                            Id = x.Id,
                            LockerRange =
                                !x.Lockers.Any()
                                    ? "0"
                                    : (x.Lockers.Min(l => l.Number) == x.Lockers.Max(l => l.Number)
                                        ? x.Lockers.Min(l => l.Number).ToString()
                                        : x.Lockers.Min(l => l.Number) + "-" + x.Lockers.Max(l => l.Number))
                        })
                        .ToList()
                })
                .ToList();

            return result;
        }



        public FurchaBLL.Models.ModuleResult GetlockersById(int id)
        {
            var module = Db.BrainModules
                .Include(x => x.Group)
                .Include(x => x.Lockers)
                .FirstOrDefault(x => x.Id == id);

            if (module == null)
                throw new ArgumentException($"Module with Id {id} not found");

            var lockers = module.Lockers
                .Where(l => !l.IsDeleted)
                .OrderBy(l => l.Number)
                .ToList();

            int lockerType = lockers.FirstOrDefault()?.LockerType ?? 0;

            int start = 0, end = 0;
            if (lockers.Any())
            {
                start = (int)(lockers.First().Number ?? 0);
                end = (int)(lockers.Last().Number ?? 0);
            }

            return new FurchaBLL.Models.ModuleResult
            {
                LockerType = lockerType,
                LockerGroupId = module.Group?.Id ?? 0,
                BranchId = module.BranchId,
                LockerRange = new LockerRange
                {
                    Start = start,
                    End = end
                }
            };
        }
        public bool UpdateModule(int lockerType, int lockerFrom, int lockerTo, int lockerGroupId, int id)
        {
            var strategy = Db.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using var transaction = Db.Database.BeginTransaction();
                try
                {
                    var module = Db.BrainModules
                        .Include(m => m.Lockers)
                        .FirstOrDefault(x => x.Id == id);

                    if (module == null)
                        throw new Exception("Module not found");

                    // restore lockers of this module
                    var moduleLockers = module.Lockers
                        .OrderBy(l => l.Id)
                        .ToList();

                    if (!moduleLockers.Any())
                        throw new Exception("Module has no lockers");

                    foreach (var locker in moduleLockers)
                    {
                        locker.IsDeleted = false;
                    }

                    var oldGroupId = module.GroupId;
                    var isUnassigned = lockerGroupId == 0;

                    // 1. Move to "no group" (unassigned) → number 1..N inside module
                    if (isUnassigned)
                    {
                        int num = 1;
                        foreach (var locker in moduleLockers)
                        {
                            locker.Number = num++;
                            locker.LockerType = lockerType;
                        }

                        // Renumber old group without this module
                        if (oldGroupId.HasValue)
                        {
                            var oldGroupLockers = Db.Lockers
                                .Include(l => l.Brain)
                                .Where(l => l.Brain.GroupId == oldGroupId
                                            && l.BrainId != module.Id
                                            && !l.IsDeleted)
                                .OrderBy(l => l.Number)
                                .ThenBy(l => l.Id)
                                .ToList();

                            int n = 1;
                            foreach (var l in oldGroupLockers)
                                l.Number = n++;
                        }

                        module.GroupId = null;

                        Db.SaveChanges();
                        transaction.Commit();
                        return true;
                    }

                    // 2. Validate target group & branch
                    var targetGroup = Db.LockerGroups.FirstOrDefault(g => g.Id == lockerGroupId);
                    if (targetGroup == null)
                        throw new Exception("Locker group not found");

                    if (module.BranchId != targetGroup.BranchId)
                        throw new BaseException(ErrorCodeEnum.GenericErrorRetry,
                            "Brain can be added only to a group inside the same branch");

                    // 3. Renumber OLD group (excluding this module)
                    if (oldGroupId.HasValue && oldGroupId.Value != lockerGroupId)
                    {
                        var oldGroupLockers = Db.Lockers
                            .Include(l => l.Brain)
                            .Where(l => l.Brain.GroupId == oldGroupId
                                        && l.BrainId != module.Id
                                        && !l.IsDeleted)
                            .OrderBy(l => l.Number)
                            .ThenBy(l => l.Id)
                            .ToList();

                        int n = 1;
                        foreach (var l in oldGroupLockers)
                            l.Number = n++;
                    }

                    // 4. Lockers of TARGET group (other modules)
                    var targetGroupOtherLockers = Db.Lockers
                        .Include(l => l.Brain)
                        .Where(l => l.Brain.GroupId == lockerGroupId
                                    && l.BrainId != module.Id
                                    && !l.IsDeleted)
                        .OrderBy(l => l.Number)
                        .ThenBy(l => l.Id)
                        .ToList();

                    // a) type compatibility:
                    //    - allow: all same
                    //    - allow: mix Common(3) + Personal(4)
                    //    - forbid: any other combination
                    var existingTypes = targetGroupOtherLockers
                        .Select(l => l.LockerType)
                        .Distinct()
                        .ToList();

                    if (existingTypes.Any())
                    {
                        const int commonId = 3;
                        const int personalId = 4;

                        var allTypes = existingTypes
                            .Append(lockerType)
                            .Distinct()
                            .ToList();

                        bool allSame = allTypes.Count == 1;
                        bool onlyCommonAndPersonal = allTypes.All(t => t == commonId || t == personalId);

                        if (!allSame && !onlyCommonAndPersonal)
                        {
                            throw new BaseException(
                                ErrorCodeEnum.GenericErrorRetry,
                                "Only 'Common' and 'Personal' locker types can be mixed in one group. " +
                                "Other types must be used alone in a group.");
                        }
                    }

                    // b) range size
                    int moduleLockerCount = moduleLockers.Count;
                    bool hasExplicitRange = lockerFrom > 0 && lockerTo > 0;

                    if (hasExplicitRange)
                    {
                        if (lockerTo - lockerFrom + 1 < moduleLockerCount)
                            throw new BaseException(ErrorCodeEnum.GenericErrorRetry,
                                "Wrong locker numbers range");

                        int moduleEnd = lockerFrom + moduleLockerCount - 1;
                        if (moduleEnd > 999)
                            throw new BaseException(ErrorCodeEnum.GenericErrorRetry,
                                "Locker numbers must be between 1 and 999");
                    }

                    // set type for module lockers
                    foreach (var locker in moduleLockers)
                    {
                        locker.LockerType = lockerType;
                        locker.IsDeleted = false;
                    }

                    // 5. Renumber TARGET group
                    if (!hasExplicitRange)
                    {
                        // No explicit range → pack all lockers 1..N (auto mode)
                        var all = targetGroupOtherLockers
                            .Concat(moduleLockers)
                            .OrderBy(l => l.BrainId)
                            .ThenBy(l => l.Id)
                            .ToList();

                        int n = 1;
                        foreach (var l in all)
                            l.Number = n++;
                    }
                    else
                    {
                        // EXPLICIT RANGE (manual edit):
                        // - DO NOT touch numbers of other modules
                        // - FAIL if there is any overlap with existing numbers

                        int moduleStart = lockerFrom;
                        int moduleEnd = lockerFrom + moduleLockerCount - 1;

                        // check overlap with existing lockers
                        bool conflict = targetGroupOtherLockers.Any(l =>
                            l.Number >= moduleStart && l.Number <= moduleEnd);

                        if (conflict)
                        {
                            throw new BaseException(
                                ErrorCodeEnum.GenericErrorRetry,
                                "Locker numbers in this range are already used in the group. " +
                                "Please choose a free range.");
                        }

                        // assign reserved range only to this module
                        for (int i = 0; i < moduleLockerCount; i++)
                        {
                            moduleLockers[i].Number = moduleStart + i;
                        }
                        // other lockers keep their numbers
                    }

                    // finally, move module to target group
                    module.GroupId = lockerGroupId;

                    Db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            });
        }


        public bool DeleteModule(int moduleId)
        {
            var module = Db.BrainModules.Include(m => m.Lockers).First(x => x.Id == moduleId);

            module.Status = 1;
            module.GroupId = null;

            foreach (var locker in module.Lockers)
            {
                locker.LockerType = 1;
                locker.IsDeleted = true;
                Db.Update(locker);
            }

            Db.Update(module);
            Db.SaveChanges();

            return true;
        }

        public bool EditModule(int moduleId, int branchId)
        {
            var module = Db.BrainModules.Include(m => m.Lockers).First(x => x.Id == moduleId);

            module.BranchId = branchId;
            module.GroupId = null;

            foreach (var locker in module.Lockers)
            {
                locker.IsDeleted = false;
                locker.LockerType = 1;
                Db.Update(locker);
            }

            Db.Update(module);
            Db.SaveChanges();

            return true;
        }


        public FurchaBLL.Models.EditModuleResult GetModuleById(int id)
        {
            var module = Db.BrainModules.FirstOrDefault(x => x.Id == id);

            if (module == null)
                throw new ArgumentException($"Module with Id {id} not found");

            return new FurchaBLL.Models.EditModuleResult
            {
                Id = id,
                BrainUid = module.BrainUid,
                BranchId = (int)module.BranchId
            };

        }

        public FurchaBLL.Models.LockerGroupResult GetLockerGroup(int id)
        {
            var group = Db.LockerGroups.FirstOrDefault(m => m.Id == id);

            return new FurchaBLL.Models.LockerGroupResult
            {
                Id = group.Id,
                Name = group.Name
            };
        }

        public bool EditGroup(int id, string name)
        {
            var group = Db.LockerGroups.FirstOrDefault(m => m.Id == id);

            group.Name = name;

            Db.SaveChanges();

            return true;
        }

    }
}
