using Domain.Entities;
using Domain.Enums;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using MqttService.Application.Models.MqttRequest;
using System.Net.Mail;
using System.Transactions;

namespace FurchaAdminApi.Services
{
    public class LockerService
    {
        private readonly LockerRepository _lockerRepository;
        private readonly BranchRepository _branchRepository;

        public LockerService(LockerRepository lockerRepository, BranchRepository branchRepository)
        {
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
        }
        /// <summary>
        /// Each user in the group should have access to same lockers in the lockerGroup. <br></br>
        /// Each locker is in only one lockerGroup.
        /// </summary>
        /// <param name="ugId">The ID of the user group.</param>
        /// <returns>A list of permitted lockers for the user group.</returns>
        public List<Locker> GetPermittedLockersOfUserGroup(int ugId)
        {
            var permittedLockers = _lockerRepository.GetPermittedLockersOfUserGroup(ugId);

            return permittedLockers ?? new List<Locker>();
        }

        /// <summary>
        /// Retrieves lockers based on specified filtering criteria.
        /// </summary>
        /// <param name="lockerType">The type of locker.</param>
        /// <param name="lockerGroupId">The locker group ID.</param>
        /// <param name="branchId">The branch ID.</param>
        /// <param name="status">The status of the locker.</param>
        /// <param name="isActive">Indicates if the locker is active.</param>
        /// <param name="lockerStatus">The current status of the locker.</param>
        /// <returns>A list of lockers that match the specified criteria.</returns>
        public List<OfficeResult> GetLockersByFilters(string? lockerType, int? lockerGroupId, int? branchId, string? status, bool? isActive)
        {
            var lockers = _lockerRepository.GetLockersByCriteria(lockerType, lockerGroupId, branchId, status, isActive);

            var adminBranches = _branchRepository.GetBranchesByAdminId(8);

            var results = new List<OfficeResult>();

            foreach (var branch in adminBranches)
            {
                var branchLockerGroups = _lockerRepository.GetLockerGroupsByBranchId(branch.Id);

                var lockerResults = new List<LockersResult>();

                foreach (var group in branchLockerGroups)
                {
                    var groupLockers = lockers.Where(locker => locker.groupid == group.Id).ToList();

                    if (groupLockers.Any())
                    {
                        lockerResults.Add(new LockersResult
                        {
                            GroupName = group.Name,
                            GroupLockers = groupLockers.Select(locker => new Locker
                            {
                                Id = locker.Id,
                                LockerType = locker.LockerType,
                                IsActive = locker.IsActive,
                                IsOpen = locker.IsOpen,
                                //    Status = locker.Status,
                                groupid = locker.groupid
                            }).ToList()
                        });
                    }
                }

                if (lockerResults.Any())
                {
                    results.Add(new OfficeResult
                    {
                        OfficeName = branch.Name,
                        Lockers = lockerResults
                    });
                }
            }

            return results;
        }

        public List<LockersResult> GetGroupsWithLockers(int branchId)
        {
            var lockerGroups = _lockerRepository.GetLockerGroupsByBranchId(branchId);
            var lockers = _lockerRepository.GetLockersByBranchId(branchId);

            var results = lockerGroups
                .Select(group => new LockersResult
                {
                    GroupName = group.Name,
                    GroupLockers = lockers.Where(locker => locker.groupid == group.Id).ToList()
                })
                .Where(result => result.GroupLockers.Any())
                .ToList();

            return results;
        }

        /// <summary>
        /// Retrieves lockers based on specified filtering criteria.
        /// </summary>
        public List<ModuleResult> GetModules()
        {
#warning auth
            var adminBranches = _branchRepository.GetBranchesByAdminId(8);

            var result = new List<ModuleResult>();
            int currentNumber = 1;

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
                            var moduleCount = _lockerRepository.GetLockersByBrainId(module.BranchId).Count;
                            moduleLockers.Add(new ModuleLockers
                            {
                                FirstLocker = currentNumber,
                                LastLocker = currentNumber + moduleCount + 1
                            });

                            currentNumber += moduleCount;
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



#warning needs to be changed to MQTT
        internal bool CreateModule(CreateModuleRequest request)
        {
            //using (var transaction = new TransactionScope())
            //{
            var module = new BrainModule
            {
                BranchId = request.BranchId,
                GroupId = request.LockerGroupId,
                MacAddress = request.MacAddress,
            };

            _lockerRepository.CreateModule(module);

            for (int i = request.FirstLocker; i < request.LastLocker; i++)
            {
                var locker = new Locker
                {
                    number = i,
                    LockerType = request.LockerType.ToString(),
                    PasswordHash = "default",
                    BranchId = request.BranchId,
                    groupid = request.LockerGroupId
                };

                _lockerRepository.CreateLocker(locker);
            }

            //}


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
    }
}
