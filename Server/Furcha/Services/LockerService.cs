using Domain.Entities;
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

        public LockerService(LockerRepository lockerRepository)
        {
            _lockerRepository = lockerRepository;
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
        public List<OfficeResult> GetLockersByFilters(int? lockerType, int? lockerGroupId, int? branchId, string? status, bool? isActive, string? lockerStatus)
        {
            var lockers = _lockerRepository.GetLockersByCriteria(lockerType, lockerGroupId, branchId, status, isActive, lockerStatus);

            var res = lockers.GroupBy(l => l.BranchId).ToList();

          //  var results = res.Select(branchGroup => new OfficeResult
          //  {
          //      OfficeName = $"Branch {branchGroup.Key}", // Replace with branch name if available
          //      Lockers = branchGroup
          //.GroupBy(l => l.GroupId)
          //.Select(group => new Locker
          //{
          //    GroupName = $"LG{group.Key} - Floor Lockers", // Replace with actual group name if available
          //    GroupLockers = group.Select(locker => new Locker
          //    {
          //        Id = locker.Id,
          //        LockerType = locker.LockerType.ToString(), // Convert to a string representation
          //        IsActive = locker.IsActive == 1,
          //        IsOpen = locker.IsOpen == 1,
          //        LockerStatus = locker.Status.ToString(), // Map status to string
          //        GroupId = locker.GroupId
          //    }).ToList()
          //}).ToList()
          //  }).ToList();

            return new List<OfficeResult>();
        }

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

            //  List<Locker> lockers = new List<Locker>();
            //for (int i = request.MinNumber; i <= request.MaxNumber; i++)
            //{
            //    lockers[i].Number = i;
            //}

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
