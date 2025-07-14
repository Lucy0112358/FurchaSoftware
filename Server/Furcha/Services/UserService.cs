using BLL.Services;
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
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly AdminRepository _adminRepository;
        private readonly LockerService _lockerService;
        private readonly LockerRepository _lockerRepository;
        private readonly BranchRepository _branchRepository;
        private readonly IMqttApiService _mqttService;

        public UserService(UserRepository userRepository, AdminRepository adminRepository, LockerService lockerService, LockerRepository lockerRepository, BranchRepository branchRepository, IMqttApiService mqttService)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _lockerService = lockerService;
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _mqttService = mqttService;
        }

        private UserResult MapUserToUserResult(User user)
        {
            var roleName = user.Role.ToString();

            // Gather user data
            var cards = _userRepository.GetUserCards(user.Id);
            var groups = _userRepository.GetUserGroupsByUserId(user.Id);
            var branches = _userRepository.GetUserBranchesByUserId(user.Id);

            return new UserResult
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Role = roleName, // This is now a string
                State = user.State.ToString(),
                Cards = cards.Select(card => new CardResult { Id = card.Id, CardNumber = card.CardNumber }).ToList(),
                UserGroups = groups.Select(group => new UserGroupResult { Id = group.Id, Name = group.Name }).ToList(),
                Branches = branches.Select(branch => new BranchResult { Id = branch.Id, Name = branch.Name }).ToList()
            };
        }

        public List<UserResult> GetFilteredUsersByPagination(int adminId, int? filterByGroupId = null, int? filterByBranchId = null, int pageNumber = 1, int pageSize = 10)
        {
            // Get all users based on admin's role
            var users = GetUsersForAdminBasedOnRole(adminId);

            // Get user IDs
            var userIds = users.Select(x => x.Id).ToList();

            // Filter users by branch and group using the repository method
            var filteredUsers = _userRepository.GetUsersByBranchGroupAndUserIds(userIds, filterByBranchId, filterByGroupId);

            // Apply pagination
            var pagedUsers = filteredUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Map to UserResult
            var userResults = pagedUsers.Select(user =>
            {


                // Get related cards, groups, and branches for the user
                var cards = _userRepository.GetUserCards(user.Id);
                var groups = _userRepository.GetUserGroupsByUserId(user.Id);
                var branches = _userRepository.GetUserBranchesByUserId(user.Id);

                // Return a new UserResult with all related data
                return new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = RoleEnum.user.ToString(),
                    State = user.State.ToString(),
                    Cards = cards.Select(card => new CardResult { Id = card.Id, CardNumber = card.CardNumber }).ToList(),
                    UserGroups = groups.Select(group => new UserGroupResult { Id = group.Id, Name = group.Name }).ToList(),
                    Branches = branches.Select(branch => new BranchResult { Id = branch.Id, Name = branch.Name }).ToList()
                };
            }).ToList();

            return userResults;
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
            var admin = _userRepository.GetAdminById(adminId);

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
            else if (admin.Role != RoleEnum.user)
            {
                users = GetUsersForCommonAdmin(admin);
            }
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            return users.Select(user => MapUserToUserResult(user)).ToList();
        }


        public List<User> GetUsersForLVL5Admin(int companyId)
        {
            return _userRepository.GetCompanyUsers(companyId);
        }

        public List<User> GetUsersForLVL4Admin(Administrator admin)
        {
            var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();

            var users = _userRepository.GetUsersByBranches(distinctBranchIds);

            return users;
        }

        public List<User> GetUsersForCommonAdmin(Administrator admin)
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
            else if (admin.Role != RoleEnum.user) //if a role is added this condition may change
            {
                // later find a smarter way not to repeat this piece of code in 4 places, DRY
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

            var groupResults = new List<UserGroupResult>();

            foreach (var group in groups)
            {
                var companyName = _branchRepository.GetCompanyById(group.CompanyId).Name;
                var groupBranches = _branchRepository.GetBranchesOfUserGroup(group.Id);
                var permittedLockers = _lockerService.GetPermittedLockersOfUserGroup(ugId: group.Id);
                var permittedLockerResults = new List<PermittedLockerResult>();
                foreach (var permittedLocker in permittedLockers)
                {
                    var permittedLockerResult = new PermittedLockerResult()
                    {
                        LockerId = permittedLocker.Id,
                        LockerNumber = permittedLocker.number
                    };

                    permittedLockerResults.Add(permittedLockerResult);
                }

                var lockerGroups = _lockerRepository.GetLockerGroupsByUserGroup(group.Id);
                var lockerGroupResults = new List<LockerGroupResult>();
                foreach (var item in lockerGroups)
                {
                    var res = new LockerGroupResult()
                    {
                        LockerGroupName = item.Name
                    };

                    lockerGroupResults.Add(res);
                }

                var userGroupResult = new UserGroupResult
                {
                    Id = group.Id,
                    Name = group.Name,
                    PermittedLockers = lockerGroupResults,
                    BranchNames = groupBranches.Select(item => item.Name).ToList(),
                    State = group.State.ToString(),
                };

                groupResults.Add(userGroupResult);
            }

            return groupResults;
        }

        public List<UserResult> SearchUsersOfAdmin(string name, int adminId)
        {

            var users = GetUsersForAdminBasedOnRole(adminId);
            var filteredUsers = users
                  .Where(u => u.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
                           || u.Surname.Contains(name, StringComparison.OrdinalIgnoreCase))
                  .ToList();

            return filteredUsers;
        }

        public async Task<UserResult> AddUser(UserCreateRequest newUser)
        {
            if (newUser.IsPinRequired == true)
            {
                // TODO: Generate a 4-digit PIN unique within the branch
            }

#warning Add columns in DB for ActiveTo and ActiveFrom. Determine user STATE based on that. Include this logic in GetAllUsersOfAdmin.

            var result = _userRepository.AddUser(newUser);

            var mqttRequest = new MqttBaseRequest<UserResult>
            {
                Command = (int)CommandTypes.CreateUserFromAdmin,
                ReceivedDate = DateTime.Now,
                Data = new List<UserResult> { result }
            };

            await _mqttService.PublishAsync(mqttRequest, "6", "1"); // Use claims or context for real companyId and branchId

            return result;
        }


        public UserGroupResult AddUserGroup(UserGroupRequest userGroupRequest)
        {
            var group = _userRepository.AddUserGroup(userGroupRequest);

            var result = new UserGroupResult
            {
                PermittedLockers = new List<LockerGroupResult>(),
                Name = group.Name,
                BranchNames = new List<string>(),
                State = group.State.ToString(),
                Id = group.Id
            };

            return result;
        }


    }
}
