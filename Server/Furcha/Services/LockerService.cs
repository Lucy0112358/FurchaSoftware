using Domain.Configuration;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
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
        private readonly IMqttService _mqttService;

        public LockerService(LockerRepository lockerRepository, BranchRepository branchRepository, UserRepository userRepository, IMqttService mqttService, furchaContext db)
        {
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _mqttService = mqttService;
            Db = db;
        }

        public bool DeleteModuleById(int id)
        {
            var module = Db.BrainModules
                .Include(b => b.Lockers)
                .FirstOrDefault(b => b.Id == id);

            if (module == null)
                return false;

            // remove dependent lockers first (FK safety)
            if (module.Lockers != null && module.Lockers.Any())
            {
                Db.Lockers.RemoveRange(module.Lockers);
            }

            Db.BrainModules.Remove(module);
            Db.SaveChanges();

            return true;
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


        /// <summary>
        /// Retrieves editingLockers based on specified filtering criteria.
        /// </summary>
        /// <param name="lockerType">The type of locker.</param>
        /// <param name="lockerGroupId">The locker mLockers ID.</param>
        /// <param name="branchId">The branch ID.</param>
        /// <param name="status">The status of the locker.</param>
        /// <param name="isActive">Indicates if the locker is active.</param>
        /// <param name="lockerStatus">The current status of the locker.</param>
        /// <returns>A list of editingLockers that match the specified criteria.</returns>
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
                // 2. Load lockers for this branch
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
                    var groupLockers = lockers.Where(l => l.groupid == group.Id).ToList();
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
                        GroupLockers = moduleGroup.ToList()
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
                .Where(l => l.Brain != null && l.Brain.BranchId == branchId)
                .Select(l => new LockerWithUsers
                {
                    Id = l.Id,
                    groupid = l.Brain.GroupId ?? 0,              // if GroupId is nullable
                    number = (long)(l.Number ?? 0),
                    LockerType = l.LockerTypeNavigation,
                    IsActive = l.IsActive ?? 0,                  // instead of (int)l.IsActive
                    Status = l.LockerStatus ?? 0,                // instead of (int)l.LockerStatus
                    IsOpen = (l.IsOpen ?? 0) == 1 ? 1 : 0,       // if IsOpen is nullable
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
                        .ToList()
                })
                .Where(result => result.GroupLockers.Any())
                .ToList();

            return results;
        }


        public List<NewModulesResult> GetNewModules(int adminId)
        {
            var companyId = Db.Administrators
                .Where(a => a.Id == adminId)
                .Select(a => a.CompanyId)
                .FirstOrDefault(); // _userRepository.GetCompanyIdByAdminId(AdminId);

            // var lockerGroups = _lockerRepository.GetBrainModulesByBranchAndStatus(4, 1);
            var brainModules = Db.BrainModules
              .Where(bm => bm.CompanyId == companyId && bm.Status == (int)BrainStatuses.New)
              .ToList();

            var results = brainModules.Select(b => new NewModulesResult
            {
                Info = b.Description,
                Id = b.Id,
                MacAddress = b.MacAddress,
                BrainUid = b.BrainUid
            }).ToList();

            return results;
        }

        public ModuleLockers GetLockersRange(int groupId)
        {
            var numbers = Db.Lockers
         .Where(l => l.Brain.GroupId == groupId && l.Number != null)
         .Select(l => l.Number.Value)
         .ToList();

            return numbers.Any()
                ? new ModuleLockers
                {
                    FirstLocker = (int)numbers.Min(),
                    LastLocker = (int)numbers.Max()
                }
                : new ModuleLockers { FirstLocker = 0, LastLocker = 0 };
        }

        /// <summary>
        /// Retrieves editingLockers based on specified filtering criteria.
        /// </summary>
        /*     public List<Models.Result.ModuleResult> GetModules(int adminId)
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
             .Where(l => l.BrainId == module.Id)
             .ToList();
                                 //_lockerRepository.GetLockersByBrainId(module.Id);
                                 if (!lockers.Any()) continue;
                                 moduleLockers.Add(new ModuleLockers
                                 {
                                     FirstLocker = (int)lockers.First().Number,
                                     LastLocker = (int)lockers.Last().Number,
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
     */
        internal bool CreateModule(ModuleRequest request)
        {
            var module = Db.BrainModules
                .Include(m => m.Lockers)
                .FirstOrDefault(m => m.Id == request.BrainId);

            if (module == null)
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "Module not found");

            module.BranchId = request.BranchId;
            module.Status = 2;

            int lockerNumber = 1;

            foreach (var locker in module.Lockers.OrderBy(l => l.Id))
            {
                locker.IsDeleted = false;
                locker.LockerType = 1;
                locker.Number = lockerNumber++;
            }

            Db.SaveChanges();
            return true;
        }


        internal bool CreateLockerGroup(int branchId, string name)
        {
            var existsWithSameName = Db.LockerGroups
                .Any(ug => ug.BranchId == branchId && ug.Name == name);

            if (existsWithSameName)
            {
                throw new BaseException(ErrorCodeEnum.LockerGroupNameExists, "A locker group with this name already exists.");
            }

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

        public ApiResult<EditLockerResult> EditLocker(List<int> lockerIds, int typeId)
        {
            var result = new EditLockerResult();
            var strategy = Db.Database.CreateExecutionStrategy();

            // Resolve ids for Common + Personal
            var allowedPair = Db.LockerTypes
                .Where(t => t.Type == "common" || t.Type == "personal")
                .Select(t => new { t.Id, t.Type })
                .ToList();

            var commonId = allowedPair.FirstOrDefault(x => x.Type == "common")?.Id;
            var personalId = allowedPair.FirstOrDefault(x => x.Type == "personal")?.Id;

            if (commonId == null || personalId == null)
                return ApiResult<EditLockerResult>.ErrorResult("Common/Personal locker types not configured.");

            if (!Db.LockerTypes.Any(t => t.Id == typeId))
                return ApiResult<EditLockerResult>.ErrorResult("Locker type not found");

            var selectedLockers = Db.Lockers
                .Include(l => l.Brain)
                .Where(l => lockerIds.Contains(l.Id))
                .ToList();

            var groups = selectedLockers
                .GroupBy(l => l.Brain.GroupId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToHashSet());

            foreach (var grp in groups)
            {
                strategy.Execute(() =>
                {
                    using var tx = Db.Database.BeginTransaction();
                    try
                    {
                        var groupId = grp.Key;
                        var selectedIdsInGroup = grp.Value;

                        var allGroupLockers = Db.Lockers
                            .Include(l => l.Brain)
                            .Where(l => l.Brain.GroupId == groupId)
                            .ToList();

                        var finalTypes = allGroupLockers
                            .Select(l => selectedIdsInGroup.Contains(l.Id) ? typeId : l.LockerType)
                            .Distinct()
                            .ToList();

                        bool ok =
                            finalTypes.Count == 1 ||
                            (
                                finalTypes.Count == 2 &&
                                finalTypes.All(t =>
                                    t == commonId.Value ||
                                    t == personalId.Value
                                )
                            );

                        if (!ok)
                            throw new BaseException(
                                ErrorCodeEnum.GenericErrorRetry,
                                "Only Common + Personal can be mixed in one group."
                            );

                        foreach (var l in allGroupLockers.Where(l => selectedIdsInGroup.Contains(l.Id)))
                            l.LockerType = typeId;

                        Db.SaveChanges();
                        tx.Commit();

                        result.SuccessfulGroups.Add((int)groupId);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        result.FailedGroups.Add(((int)grp.Key, ex.Message));
                    }
                });
            }

            return ApiResult<EditLockerResult>.Success(result);
        }



        public void SuspendLockers(List<int> lockerIds, int type)
        {
            foreach (var id in lockerIds)
            {
                var locker = Db.Lockers.FirstOrDefault(l => l.Id == id);
                if (locker == null) continue;

                locker.IsActive = type;
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

                await _mqttService.PublishAsync(mqttRequest, $"controller/{companyUid}/{brainLockers.Key}");
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

                if (locker.LockerType == 4) // personal
                {
                    locker.LockerStatus = 2;
                }
            }

            Db.SaveChanges();
        }

        public List<AllModulesResult> GetAddedModules(int adminId)
        {
            var result = Db.Branches
                .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId))
                    .Include(b => b.BrainModules)
                        .ThenInclude(m => m.Group)
                .Select(b => new AllModulesResult
                {
                    BranchName = b.Name,
                    Modules = b.BrainModules
                        .Where(m => m.Status == 2)
                        .Select(m => new BranchModules
                        {
                            Id = m.Id,
                            Info = m.Description,
                            GroupName = m.Group.Name,
                            LockerRange = m.Lockers.Any()
                                ? (
                                    m.Lockers.Min(l => l.Number) == m.Lockers.Max(l => l.Number)
                                        ? m.Lockers.Min(l => l.Number).ToString()
                                        : m.Lockers.Min(l => l.Number) + "-" + m.Lockers.Max(l => l.Number)
                                  )
                                : "0"
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
                .Include(x => x.Lockers) // ensure module.Lockers is loaded
                .FirstOrDefault(x => x.Id == id);

            if (module == null)
                throw new ArgumentException($"Module with Id {id} not found");

            int groupId = module.Group?.Id ?? 0;

            var firstLockerType = module.Lockers?.FirstOrDefault()?.LockerType
       ?? module.Lockers?.FirstOrDefault()?.LockerType;

            /*  if (firstLockerType == null)
                  throw new InvalidOperationException($"No lockers found for module {id}");*/

            return new FurchaBLL.Models.ModuleResult
            {
                LockerType = firstLockerType,
                LockerGroupId = module.Group?.Id ?? 0,
                BranchId = module.BranchId ?? 0,
                LockerRange = module.Lockers.Any()
                    ? new LockerRange
                    {
                        Start = (int)module.Lockers.Min(l => l.Number ?? 0),
                        End = (int)module.Lockers.Max(l => l.Number ?? 0)
                    }
                    : new LockerRange { Start = 0, End = 0 }
            };
        }

        public bool UpdateModule(
      int lockerType,
      int lockerFrom,
      int lockerTo,
      int lockerGroupId,
      int id)
        {
            var strategy = Db.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using var transaction = Db.Database.BeginTransaction();
                try
                {
                    var module = Db.BrainModules
                        .Include(m => m.Lockers)
                        .Include(m => m.Group)
                        .FirstOrDefault(x => x.Id == id);

                    if (module == null)
                        throw new Exception("Module not found");

                    var oldGroupId = module.GroupId;

                    if (oldGroupId.HasValue && oldGroupId.Value != lockerGroupId)
                    {
                        var lockersOfOldGroup = Db.Lockers
                            .Include(l => l.Brain)
                            .Where(l => l.Brain.GroupId == oldGroupId.Value)
                            .OrderBy(l => l.Number)
                            .ToList();

                        for (int i = 0; i < lockersOfOldGroup.Count; i++)
                            lockersOfOldGroup[i].Number = i + 1;
                    }

                    var lockersOfNewGroup = Db.Lockers
                        .Include(l => l.Brain)
                        .Where(l => l.Brain.GroupId == lockerGroupId)
                        .ToList();

                    var lockersToUpdate = module.Lockers
                        .OrderBy(l => l.Id)
                        .ToList();

                    var lockerCount = lockersToUpdate.Count;

                    if (lockerTo - lockerFrom + 1 < lockerCount)
                        throw new Exception("Invalid locker range");

                    var lockerIdsToUpdate = lockersToUpdate
                        .Select(l => l.Id)
                        .ToHashSet();

                    var existingNumbers = lockersOfNewGroup
                        .Where(l => !lockerIdsToUpdate.Contains(l.Id))
                        .Select(l => l.Number)
                        .ToHashSet();

                    for (int num = lockerFrom; num <= lockerTo; num++)
                    {
                        if (existingNumbers.Contains(num))
                            throw new Exception($"Locker number {num} already exists in the group");
                    }

                    for (int i = 0; i < lockerCount; i++)
                    {
                        lockersToUpdate[i].Number = lockerFrom + i;
                        lockersToUpdate[i].LockerType = lockerType;
                    }

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
            var module = Db.BrainModules
                .Include(m => m.Lockers)
                .First(x => x.Id == moduleId);

            module.Status = 1;
            module.GroupId = null;

            var lockerIds = module.Lockers.Select(l => l.Id).ToList();

            if (lockerIds.Count > 0)
            {
                Db.Database.ExecuteSqlRaw(
                    $"DELETE FROM furcha.UserLocker WHERE LockerId IN ({string.Join(",", lockerIds)})");

                var groupLockers = Db.UserGroupLockers
                .Where(x => lockerIds.Contains(x.LockerId))
                .ToList();

                Db.UserGroupLockers.RemoveRange(groupLockers);

            }

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
            var module = Db.BrainModules
                .Include(m => m.Lockers)
                .First(x => x.Id == moduleId);

            module.BranchId = branchId;
            // DO NOT touch GroupId here

            foreach (var locker in module.Lockers)
            {
                locker.IsDeleted = false;
                locker.LockerType = 1;
            }

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
                BranchId = (int)module.BranchId,
                Info = module.Description
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
            name = name?.Trim();

            var group = Db.LockerGroups.FirstOrDefault(m => m.Id == id);

            var existsWithSameName = Db.LockerGroups
                .Any(ug => ug.BranchId == group.BranchId && ug.Name == name && ug.Id != id);

            if (existsWithSameName)
            {
                throw new BaseException(ErrorCodeEnum.LockerGroupNameExists, "A locker group with this name already exists.");
            }

            group.Name = name;

            Db.SaveChanges();

            return true;
        }

        public bool DeleteModulePermanently(int moduleId)
        {
            var module = Db.BrainModules
                .Include(m => m.Lockers)
                .FirstOrDefault(m => m.Id == moduleId);

            if (module == null)
                throw new Exception("Module not found");

            Db.Lockers.RemoveRange(module.Lockers);
            Db.BrainModules.Remove(module);

            Db.SaveChanges();
            return true;
        }


        public bool DeleteLockerGroup(int groupId)
        {
            var strategy = Db.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using var transaction = Db.Database.BeginTransaction();
                try
                {
                    var group = Db.LockerGroups
                        .Include(g => g.AdminLockerGroups)
                        .Include(g => g.BrainModules)
                            .ThenInclude(m => m.Lockers)
                        .FirstOrDefault(g => g.Id == groupId);

                    if (group == null)
                        return false;

                    if (group.AdminLockerGroups.Any())
                    {
                        Db.AdminLockerGroups.RemoveRange(group.AdminLockerGroups);
                    }

                    foreach (var module in group.BrainModules)
                    {
                        module.GroupId = null;

                        foreach (var locker in module.Lockers)
                        {
                            locker.IsDeleted = true;
                            locker.LockerType = 1; 
                        }
                    }

                    Db.LockerGroups.Remove(group);

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

    }
}
