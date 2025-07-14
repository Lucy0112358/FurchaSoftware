using Domain.Entities;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;

namespace FurchaAdminApi.Services
{
    public class LockerService
    {
        private readonly LockerRepository _lockerRepository;
        private readonly BranchRepository _branchRepository;
        private readonly UserRepository _userRepository;
        private readonly IMqttApiService _mqttService;

        public LockerService(LockerRepository lockerRepository, BranchRepository branchRepository, UserRepository userRepository, IMqttApiService mqttService)
        {
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _mqttService = mqttService;
        }
        /// <summary>
        /// Each user in the group should have access to same editingLockers in the lockerGroup. <br></br>
        /// Each locker is in only one lockerGroup.
        /// </summary>
        /// <param name="ugId">The ID of the user group.</param>
        /// <returns>A list of permitted editingLockers for the user group.</returns>
        public List<Locker> GetPermittedLockersOfUserGroup(int ugId)
        {
            var permittedLockers = _lockerRepository.GetPermittedLockersOfUserGroup(ugId);

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
        public List<OfficeResult> GetLockersByFilters(int branchId, string? lockerType, int? lockerGroupId, int? isOpen = null,
            string? userName = null)
        {

            var adminBranches = _branchRepository.GetBranchesByAdminId(8);

            var results = new List<OfficeResult>();

            foreach (var branch in adminBranches)
            {
                var lockers = _lockerRepository.GetLockersByCriteria(branch.Id, lockerType, lockerGroupId, isOpen);

                if (userName != null)
                {
                    /*                   .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                                             || u.Surname.Contains(name, StringComparison.OrdinalIgnoreCase))
                                    .ToList();*/
                }

                var branchLockerGroups = _lockerRepository.GetLockerGroupsByBranchId(branch.Id)
                    .OrderByDescending(group => group.Id)
                    .ToList();

                var lockerResults = new List<LockersResult>();

                foreach (var group in branchLockerGroups)
                {
                    var groupLockers = lockers
                        .Where(locker => locker.groupid == group.Id)
                        .ToList();

                    lockerResults.Add(new LockersResult
                    {
                        GroupName = group.Name,
                        GroupLockers = groupLockers.Select(locker => new LockerWithUsers
                        {
                            Id = locker.Id,
                            LockerType = locker.LockerType,
                            IsActive = locker.IsActive,
                            IsOpen = locker.IsOpen,
                            groupid = locker.groupid,
                            Users = locker.Users,
                            BranchId = locker.BranchId
                        }).ToList()
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
            var lockerGroups = _lockerRepository.GetLockerGroupsByBranchId(branchId).Distinct().ToList();
            var lockers = _lockerRepository.GetUserLockersByBranchId(branchId);

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
            var accountId = _userRepository.GetCompanyIdByAdminId(adminId);

            var lockerGroups = _lockerRepository.GetBrainModulesByBranchAndStatus(4, 1);

            var results = lockerGroups.Select(l => new NewModulesResult
            {
                Info = l.Info,
                Id = l.Id,
                MacAddress = l.MacAddress,
            }).ToList();

            return results;
        }

        public ModuleLockers GetLockersRange(int groupId)
        {
            var lockersWithNumber = _lockerRepository.GetLockersOfGroup(groupId)
                 .OrderBy(x => x.number).Select(x => x.number).ToList();

            return new ModuleLockers
            {
                FirstLocker = lockersWithNumber.FirstOrDefault(),
                LastLocker = lockersWithNumber.LastOrDefault()
            };
        }

        /// <summary>
        /// Retrieves editingLockers based on specified filtering criteria.
        /// </summary>
        public List<ModuleResult> GetModules()
        {
#warning auth
            var adminBranches = _branchRepository.GetBranchesByAdminId(8);

            var result = new List<ModuleResult>();


            foreach (var branch in adminBranches)
            {
                var branchLockerGroups = _lockerRepository.GetLockerGroupsByBranchId(branch.Id);

                var moduleInfos = new List<ModuleInfo>();

                foreach (var group in branchLockerGroups)
                {
                    var groupModules = _lockerRepository.GetModulesByGroupId(group.Id);

                    if (groupModules.Any())
                    {
                        var moduleLockers = new List<ModuleLockers>();

                        foreach (var module in groupModules)
                        {
                            var lockers = _lockerRepository.GetLockersByBrainId(module.Id);
                            if (!lockers.Any()) continue;
                            moduleLockers.Add(new ModuleLockers
                            {
                                FirstLocker = lockers.First().number,
                                LastLocker = lockers.Last().number,
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
                    result.Add(new ModuleResult
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
            if (request.LockerGroupId == null)
            {
                var newGroup = new LockerGroup
                {
                    Name = "Unassigned 1", // index names
                    BranchId = request.BranchId,
                };

                var unassignedGroup = _lockerRepository.CreateLockerGroup(newGroup);
                request.LockerGroupId = unassignedGroup.Id;
            }

            var module = new BrainModule
            {
                Id = request.Id,
                BranchId = request.BranchId,
                Status = 2,
                GroupId = request.LockerGroupId
            };

            _lockerRepository.UpdateModule(module);

            int startIndex = 1;

            var lockersWithNumber = _lockerRepository.GetLockersOfGroup((int)request.LockerGroupId)
              .OrderBy(x => x.number).ToList();

            if (request.StartBegin)
            {
                if (lockersWithNumber.Any())
                {
                    foreach (var locker in lockersWithNumber)
                    {
                        var l = new Locker
                        {
                            Id = locker.Id,
                            number = locker.number + request.LastLocker
                        };

                        _lockerRepository.UpdateLocker(l);
                    }
                }
            }
            else
            {
                if (lockersWithNumber.Any())
                {
                    startIndex = lockersWithNumber.Select(x => x.number).Max() + 1;
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
                    number = i,
                    LockerType = locker_type.Common.ToString(),
                    PasswordHash = "default",
                    BranchId = request.BranchId,
                    groupid = request.LockerGroupId,
                    BrainId = request.Id
                };

                _lockerRepository.CreateLocker(locker);
            }

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

            _lockerRepository.CreateLockerGroup(result);

            return true;
        }

        /// <summary>
        /// Retrieves all locker groups associated with a specific admin ID.
        /// </summary>
        /// <param name="adminId">The ID of the admin.</param>
        /// <returns>A list of locker groups associated with the specified admin.</returns>
        public List<LockerGroup> GetLockerGroupsByAdminId(int adminId)
        {
            var lockerGroups = _lockerRepository.GetLockerGroupsByAdminId(adminId);
            return lockerGroups;
        }

        public void EditLocker(List<int> lockerIds, string type)
        {
            var editingLockers = new List<Locker>();

            foreach (var id in lockerIds)
            {
                var locker = _lockerRepository.GetLockerByIdOrDefault(id);
                if (locker != null)
                {
                    editingLockers.Add(locker);
                }
            }

            // Safe because nulls are excluded
            var groupedLockers = editingLockers
                .GroupBy(l => l.groupid)
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

                var all = _lockerRepository.GetLockersOfGroup((int)lockerGroup.Key);
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
                    _lockerRepository.UpdateLocker(locker);
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
            // make isactive to 0, or 1
            foreach (var id in lockerIds)
            {
                var locker = _lockerRepository.GetLockerByIdOrDefault(id);
                if (locker.IsActive == 1)
                {
                    locker.IsActive = 2;
                }
                else
                {
                    locker.IsActive = 1;
                }
                locker = _lockerRepository.UpdateLocker(locker);
            }

        }

        public async void OpenLockers(List<int> lockerIds)
        {
            var mqttRequest = new MqttBaseRequest<int>
            {
                Command = (int)CommandTypes.OpenLockersFromAdmin,
                ReceivedDate = DateTime.Now,
                Data = lockerIds
            };

            await _mqttService.PublishAsync<int>(mqttRequest, "6", "1"); // take from claims
        }

        public void SetUser(List<int> lockerIds, int userId)
        {
            // make isactive to 0, or 1
            _userRepository.AssignLockersToUser(lockerIds, userId);
        }
    }
}
