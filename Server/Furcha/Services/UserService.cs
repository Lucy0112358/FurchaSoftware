using Domain.Entities;
using FurchaAdminApi.Repos;

namespace FurchaAdminApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public List<User> GetFilteredUsersWithPagination(int? filterByGroupId, int? filterByBranchId, int pageNumber, int pageSize)
        {

            return _userRepository.GetFilteredUsersByPagination(filterByGroupId, filterByBranchId, pageNumber, pageSize);
        }
    }
}
