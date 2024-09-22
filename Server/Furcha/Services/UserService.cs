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

        public List<Branch> GetAllBranchesOfCompanyByAdminId(int adminId)
        {
            var admin = _userRepository.GetAdminById(adminId);

            return _userRepository.GetAllBranchesOfCompany(admin.CompanyId);
        }

        public List<UserGroup> GetUserGroupsByAdminId(int adminId)
        {
            var admin = _userRepository.GetAdminById(adminId);

            return _userRepository.GetUserGroupsByCompanyId(admin.CompanyId);
        }
        public List<User> SearchUsers(string name)
        {
            return _userRepository.SearchUsersByName(name);
        }

    }
}
