using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;

namespace FurchaAdminApi.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly AdminRepository _adminRepository;
        private readonly LockerService _lockerService;
        private readonly LockerRepository _lockerRepository;
        private readonly BranchRepository _branchRepository;


        public UserService(UserRepository userRepository, AdminRepository adminRepository, LockerService lockerService, LockerRepository lockerRepository, BranchRepository branchRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _lockerService = lockerService;
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
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
            var users = GetUsersForAdminBasedOnRole(adminId);

            if (filterByGroupId.HasValue)
            {
                users = users.Where(user => _userRepository.IsUserInGroup(user.Id, filterByGroupId.Value)).ToList();
            }
            else if (filterByBranchId.HasValue)
            {
                users = users.Where(user => _userRepository.IsUserInBranch(user.Id, filterByBranchId.Value)).ToList();
            }

            var pagedUsers = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var userResults = pagedUsers.Select(user =>
            {
                RoleEnum parsedRole;
                // Refactor this embaressement
                if (long.TryParse(user.Role, out long roleId) && Enum.IsDefined(typeof(RoleEnum), roleId))
                {
                    parsedRole = (RoleEnum)roleId;
                    var roleName = _adminRepository.GetRoleById(parsedRole).OpenName;

                    var cards = _userRepository.GetUserCards(user.Id);
                    var groups = _userRepository.GetUserGroupsByUserId(user.Id);
                    var branches = _userRepository.GetUserBranchesByUserId(user.Id);

                    return new UserResult
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Role = roleName,
                        State = user.State.ToString(),
                        Cards = cards.Select(card => new CardResult { Id = card.Id, CardNumber = card.CardNumber }).ToList(),
                        UserGroups = groups.Select(group => new UserGroupResult { Id = group.Id, Name = group.Name }).ToList(),
                        Branches = branches.Select(branch => new BranchResult { Id = branch.Id, Name = branch.Name }).ToList()
                    };
                }
                else
                {
                    var roleName = RoleEnum.user.ToString();

                    var cards = _userRepository.GetUserCards(user.Id);
                    var groups = _userRepository.GetUserGroupsByUserId(user.Id);
                    var branches = _userRepository.GetUserBranchesByUserId(user.Id);

                    return new UserResult
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Role = roleName,
                        State = user.State.ToString(),
                        Cards = cards.Select(card => new CardResult { Id = card.Id, CardNumber = card.CardNumber }).ToList(),
                        UserGroups = groups.Select(group => new UserGroupResult { Id = group.Id, Name = group.Name }).ToList(),
                        Branches = branches.Select(branch => new BranchResult { Id = branch.Id, Name = branch.Name }).ToList()
                    };
                }
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
                        LockerNumber = permittedLocker.Number
                    };

                    permittedLockerResults.Add(permittedLockerResult);
                }

                var lockerGroups = _lockerRepository.GetLockerGroupsByUserGroup(group.Id);
                var lockerGroupResults = new List<LockerGroupResult>();
                foreach (var item in lockerGroups)
                {
                    var res = new LockerGroupResult()
                    {
                        LockerGroupName = item.LockerGroupName
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
            var adminBranches = _userRepository.GetAdminBranchesByAdminId(adminId);
            var branchIds = adminBranches.Select(b => b.Id).ToList();
            var users = _userRepository.SearchUserByAdminId(name, adminId, branchIds);

            return users.Select(user => MapUserToUserResult(user)).ToList();
        }

        public Card AddUser(UserCreateRequest newUser)
        {

            return _userRepository.AddCard(newUser);

        }
    }
}
