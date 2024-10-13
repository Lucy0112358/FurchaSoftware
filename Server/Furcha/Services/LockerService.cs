using Domain.Entities;
using FurchaAdminApi.Repos;

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
    }
}
