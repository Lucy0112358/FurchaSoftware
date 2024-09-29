using Domain.Entities;
using Domain.Enums;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using MqttService.Application.Exceptions;

namespace FurchaAdminApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly AdminRepository _adminRepository;

        public UserService(UserRepository userRepository, AdminRepository adminRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
        }

        public List<User> GetFilteredUsersWithPagination(int companyId, int? filterByGroupId, int? filterByBranchId, int pageNumber, int pageSize)
        {
            return _userRepository.GetFilteredUsersByPagination(companyId, filterByGroupId, filterByBranchId, pageNumber, pageSize);
        }

        public List<BranchFilterResult> GetAdminBranches(int adminId)
        {
            var branches = _userRepository.GetAllBranchesOfAdmin(adminId);

            var branchFilterResults = branches.Select(branch => new BranchFilterResult
            {
                Id = branch.Id,
                Name = branch.Name
            }).ToList();

            return branchFilterResults;
        }

        public List<User> GetCompanyUsers(int companyId)
        {
            var users = _userRepository.GetCompanyUsers(companyId);

            return users;
        }


        public List<UserResult> GetUsersForAdminBasedOnRole(int adminId)
        {
            var admin = _userRepository.GetAdminById(adminId: adminId);

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }
            var users = new List<User>();

            if (admin.Role == RoleEnum.LVL5_MasterAdmin)
            {
                users = GetUsersForLVL5Admin(admin.CompanyId);
            }
            else if (admin.Role == RoleEnum.LVL4_SuperAdmin)
            {
                users = GetUsersForLVL4Admin(admin);
            }
            else if (admin.Role != RoleEnum.NotSet)
            {
                users = GetUsersForCommonAdmin(admin);
            }
            else
            {
                // Not likely to happen, when user has no role of admin
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var userResults = new List<UserResult>();

            foreach (var user in users)
            {
                var roleName = _adminRepository.GetRoleById(user.Role).OpenName;
                var cards = _userRepository.GetUserCards(user.Id);
                var groups = _userRepository.GetUserGroupsByUserId(user.Id);
                var branches = _userRepository.GetUserBranchesByUserId(user.Id);

                var userResult = new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = roleName,
                    State = user.State.ToString(),

                    Cards = cards.Select(card => new CardResult
                    {
                        Id = card.Id,
                        CardNumber = card.CardNumber,
                    }).ToList(),

                    UserGroups = groups.Select(ug => new UserGroupResult
                    {
                        Id = ug.Id,
                        GroupName = ug.Name,
                    }).ToList(),

                    Branches = branches.Select(b => new BranchResult
                    {
                        Id = b.Id,
                        Name = b.Name,
                    }).ToList()
                };

                userResults.Add(userResult);
            }

                return userResults;
        }


        public List<User> GetUsersForLVL5Admin(int companyId)
        {
            return _userRepository.GetCompanyUsers(companyId);
        }

        public List<User> GetUsersForLVL4Admin(Administrators admin)
        {
            var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();

            var users = _userRepository.GetUsersByBranches(distinctBranchIds);

            return users;
        }

        public List<User> GetUsersForCommonAdmin(Administrators admin)
        {
            var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();
            if (distinctBranchIds.Count > 1)
            {
                throw new BaseException(ErrorCodeEnum.AdminHasMoreBranchesThanPermitted);
            }
            var users = _userRepository.GetAllUsersOfBranch(distinctBranchIds[0]);

            return users;
        }

        public List<UserGroupResult>? GetUserGroupsForAdminBasedOnRole(int adminId)
        {
            var admin = _userRepository.GetAdminById(adminId);

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groups = new List<UserGroup>();

            if (admin.Role == RoleEnum.LVL5_MasterAdmin)
            {
                groups = _userRepository.GetUserGroupsByCompanyId(admin.CompanyId);
            }
            else if (admin.Role == RoleEnum.LVL4_SuperAdmin)
            {
                var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

                var distinctBranchIds = adminBranches
                                 .Select(ub => ub.Id)
                                 .Distinct()
                                 .ToList();

                groups = _userRepository.GetUserGroupsByBranchIds(distinctBranchIds);
            }
            else if (admin.Role != RoleEnum.NotSet)
            {
                // later find a smarter way not to repeat this piece of code in 4 places
                var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

                var distinctBranchIds = adminBranches
                                 .Select(ub => ub.Id)
                                 .Distinct()
                                 .ToList();

                if (distinctBranchIds.Count > 1)
                {
                    throw new BaseException(ErrorCodeEnum.AdminHasMoreBranchesThanPermitted);
                }

                groups = _userRepository.GetUserGroupsByBranchIds(distinctBranchIds);
            }
            else
            {
                // Not likely to happen, when user has no role of admin
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            return groups.Select(ug => new UserGroupResult
            {
                Id = ug.Id,
                GroupName = ug.Name,
                Branch = ug.Branch.Name,
                State = ug.State,
                /*                PermittedLockers = ug.UserGroupLockerGroups
                                             .Select(ugl => ugl.)
                                             .ToList() */
            }).ToList();
        }

        public List<User> SearchUsers(string name)
        {
            return _userRepository.SearchUsersByName(name);
        }

    }
}
