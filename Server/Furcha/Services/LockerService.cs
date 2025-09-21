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
        /// Each user in the group should have access to same editingLockers in the lockerGroup. <br></br>
        /// Each locker is in only one lockerGroup.
        /// </summary>
        /// <param name="ugId">The ID of the user group.</param>
        /// <returns>A list of permitted editingLockers for the user group.</returns>
        public List<FurchaDAL.Models.Locker> GetPermittedLockersOfUserGroup(int ugId)
        {
            var permittedLockers = Db.UserGroupLockers
                .Where(ugl => ugl.UserGroupId == ugId)
                .Select(ugl => ugl.Locker)
                .ToList(); //_lockerRepository.GetPermittedLockersOfUserGroup(ugId);

            return permittedLockers ?? new List<Locker>();
        }

        /// <summary>
        /// Retrieves editingLockers based on specified filtering criteria.
        /// </summary>
        /// <param name="lockerType">The type of locker.</param>
        /// <param name="lockerGroupId">The locker group ID.</param>
        /// <param name="branchId">The branch ID.</param>
        /// <param name="status">The status of the locker.</param>
        /// <param name="isActive">Indicates if the locker is active.</param>
        /// <param name="lockerStatus">The current status of the locker.</param>
        /// <returns>A list of editingLockers that match the specified criteria.</returns>
        public List<OfficeResult> GetLockersByFilters(
           int? branchId,
           string? lockerType,
           int? lockerGroupId,
           int? isOpen = null,
           string? userName = null,
           int? adminId = null)
        {
            var adminBranches = new List<Branch>();

            if (branchId == null)
            {
                adminBranches = Db.Branches
                    .Include(b => b.AdminBranches)
                    .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId))
                    .ToList();
            }
            else
            {
                adminBranches = Db.Branches
                    .Include(b => b.AdminBranches)
                    .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId && ab.BranchId == branchId))
                    .ToList();
            }

            var results = new List<OfficeResult>();

            foreach (var branch in adminBranches)
            {
                var lockers = Db.Lockers
                    .Include(l => l.Brain)
                    .Where(l =>
                        (lockerType == null || l.LockerType == lockerType) &&
                        (lockerGroupId == null || l.Brain.GroupId == lockerGroupId) &&
                        l.Brain.BranchId == branch.Id &&
                        (isOpen == null || l.IsOpen == (isOpen == 1))
                    )
                    .Select(l => new LockerWithUsers
                    {
                        Id = l.Id,
                        number = (long)(l.Number ?? 0),
                        groupid = l.Brain.GroupId,
                        LockerType = l.LockerType,
                        IsActive = l.IsActive == true ? 1 : 0,
                        IsOpen = l.IsOpen == true ? 1 : 0,
                        BranchId = l.Brain.BranchId ?? 0,
                        PasswordHash = l.PasswordHash,
                        Users = l.Users
                            .Where(u => u != null)
                            .Select(u => u.Name)
                            .ToList()
                    })
                    .ToList();

                var branchLockerGroups = Db.LockerGroups
                    .Where(lg => lg.BranchId == branch.Id)
                    .OrderByDescending(group => group.Id)
                    .ToList();

                var lockerResults = new List<LockersResult>();

                if (branchLockerGroups.Count == 0)
                {
                    lockerResults.Add(new LockersResult
                    {
                        GroupName = "Unassigned",
                        GroupLockers = lockers
                    });

                    results.Add(new OfficeResult
                    {
                        OfficeName = branch.Name,
                        Lockers = lockerResults
                    });

                    continue;
                }

                foreach (var group in branchLockerGroups)
                {
                    var groupLockers = lockers
                        .Where(locker => locker.groupid == group.Id)
                        .ToList();

                    lockerResults.Add(new LockersResult
                    {
                        GroupName = group.Name,
                        GroupLockers = groupLockers
                    });
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
             .Where(lg => lg.BranchId == branchId).Distinct()
             .ToList(); //_lockerRepository.GetLockerGroupsByBranchId(branchId).Distinct().ToList();
                        // var lockers = _lockerRepository.GetUserLockersByBranchId(branchId);
            var lockers = Db.Lockers.Include(x => x.Brain)
                 .Where(l => l.Brain.BranchId == branchId)
                 .Select(l => new LockerWithUsers
                 {
                     Id = l.Id,
                     groupid = l.Brain.GroupId,
                     number = (long)(l.Number ?? 0),
                     LockerType = l.LockerType,
                     IsActive = l.IsActive == true ? 1 : 0,
                     IsOpen = l.IsOpen == true ? 1 : 0,
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
        .Where(l => l.Brain.GroupId == groupId).ToList() //_lockerRepository.GetLockersOfGroup(groupId)
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
        .ToList(); //_lockerRepository.GetModulesByGroupId(group.Id);

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

        internal bool CreateModule(ModuleRequest request)
        {
            /* using (var transaction = new TransactionScope())
             {*/
            /*  if (request.LockerGroupId == null)
              {
                  var newGroup = new LockerGroup
                  {
                      Name = "Unassigned 1", // index names
                      BranchId = request.BranchId,
                  };

                  var unassignedGroup = Db.LockerGroups.Add(newGroup);
                  Db.SaveChanges();
                  //_lockerRepository.CreateLockerGroup(newGroup);

                  request.LockerGroupId = unassignedGroup.Entity.Id;
              }*/

            var existingModule = Db.BrainModules.FirstOrDefault(m => m.Id == request.BrainId);
            if (existingModule != null)
            {
                existingModule.BranchId = request.BranchId;
                existingModule.Status = 2;

                Db.SaveChanges();
            }

            /*   int startIndex = 1;

               var lockersWithNumber = Db.Lockers
                   .Where(l => l.GroupId == (int)request.LockerGroupId)
                   .ToList() // _lockerRepository.GetLockersOfGroup((int)request.LockerGroupId)
                   .OrderBy(x => (int)x.Number).ToList();

               if (request.StartBegin)
               {
                   if (lockersWithNumber.Any())
                   {
                       foreach (var locker in lockersWithNumber)
                       {
                           var l = Db.Lockers.FirstOrDefault(l => l.Id == locker.Id);
                           if (l != null)
                           {
                               l.Number = locker.Number + request.LastLocker;
                               Db.SaveChanges();
                           }

                           //_lockerRepository.UpdateLocker(l);
                       }
                   }
               }
               else
               {
                   if (lockersWithNumber.Any())
                   {
                       startIndex = lockersWithNumber.Select(x => (int)x.Number).Max() + 1;
                   }
                   else
                   {
                       startIndex = 1;
                   }
               }


               if (startIndex >= request.LastLocker || request.FirstLocker < startIndex)
               {
                   throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "locker number is wrong");
               }

               for (int i = startIndex; i < request.LastLocker; i++)
               {
                   var locker = new Locker
                   {
                       Number = i,
                       LockerType = locker_type.Common.ToString(),
                       PasswordHash = "default",
                       BranchId = request.BranchId,
                       GroupId = request.LockerGroupId,
                       BrainId = request.Id
                   };

                   //  _lockerRepository.CreateLocker(locker);
                   Db.Lockers.Add(locker);
                   Db.SaveChanges();
               }
   */
            /*     }*/
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

        public void EditLocker(List<int> lockerIds, string type)
        {
            var editingLockers = new List<Locker>();

            foreach (var id in lockerIds)
            {
                var locker = Db.Lockers.Include(l => l.Brain).FirstOrDefault(l => l.Id == id);// _lockerRepository.GetLockerByIdOrDefault(id);
                if (locker != null)
                {
                    editingLockers.Add(locker);
                }
            }

            // Safe because nulls are excluded
            var groupedLockers = editingLockers
                .GroupBy(l => l.Brain.GroupId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var lockerGroup in groupedLockers)
            {
                var types = new HashSet<string>();
                if (type != "common")
                {
                    types.Add(type);
                }

                var groupId = lockerGroup.Key;
                var lockersInGroup = lockerGroup.Value;

                var all = Db.Lockers.Include(l=> l.Brain)
                    .Where(l => l.Brain.GroupId == groupId)
                    .ToList();
                //_lockerRepository.GetLockersOfGroup((int)lockerGroup.Key);
                foreach (var locker in all)
                {
                    if (!lockersInGroup.Contains(locker))
                    {
                        if (locker.LockerType != "common")
                        {
                            types.Add(locker.LockerType.ToLower());
                        }
                    }

                }
                if (types.Count > 2)
                {
                    throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "Locker types must be the same in the group");
                }
                foreach (var locker in lockersInGroup)
                {

                    if (type != null)
                    {
                        locker.LockerType = type;
                    }
                    Db.Lockers.Update(locker);
                    Db.SaveChanges();
                    // _lockerRepository.UpdateLocker(locker);
                }
            }


            /*if (locker != null)
            {
                groupedLockers.TryGetValue(locker.groupid, out string val);
                if (type != val)
                {
                    throw new Exception();
                }
                if (type != null)
                {
                    locker.LockerType = type;
                }
                _lockerRepository.UpdateLocker(locker);
            }*/


        }

        public void SuspendLockers(List<int> lockerIds)
        {
            foreach (var id in lockerIds)
            {
                var locker = Db.Lockers.FirstOrDefault(l => l.Id == id);
                if (locker == null) continue;

                locker.IsActive = locker.IsActive == true ? false : true;
            }

            Db.SaveChanges();
        }

        public async void OpenLockers(List<int> lockerIds)
        {
            var mqttRequest = new MqttBaseRequest<List<int>>
            {
                Command = (int)CommandTypes.OpenLockersFromAdmin,
                ReceivedDate = DateTime.Now,
                Data = lockerIds
            };

            await _mqttService.PublishToMqtt<List<int>>(mqttRequest, "1"); // take from claims
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
                            LockerRange = x.Lockers.Count() == 0
                                ? "0"
                                : x.Lockers.Count() == 1
                                    ? "1"
                                    : "1-" + x.Lockers.Count().ToString()
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
                LockerRange = new LockerRange
                {
                    Start = 1,
                    End = module.Lockers?.Count() ?? 0
                }
            };

        }

        public bool UpdateModule(string lockerType, int lockerFrom, int lockerTo, int lockerGroupId, int id)
        {
            var module = Db.BrainModules.Include(m => m.Lockers).FirstOrDefault(x => x.Id == id);
            module.GroupId = lockerGroupId;
            foreach (var locker in module.Lockers)
            {
                locker.LockerType = lockerType;
            }
            Db.SaveChanges();
            /*   if(module.Lockers.Count < lockerTo - lockerFrom)
               {
                   Db.Lockers.Add()
               }*/

            return true;
        }

        public bool DeleteModule(int moduleId)
        {
            var module = Db.BrainModules.Include(m => m.Lockers).First(x => x.Id == moduleId);

            module.Status = 1;
            module.GroupId = null;

            foreach(var locker in module.Lockers)
            {
                locker.LockerType = "unassigned";
                Db.Update(locker);
            }

            Db.Update(module);
            Db.SaveChanges();

            return true;
        }

    }
}
