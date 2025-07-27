using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;


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
        private readonly furchaContext db;
        

        public UserService(UserRepository userRepository, AdminRepository adminRepository, LockerService lockerService, LockerRepository lockerRepository, BranchRepository branchRepository, IMqttApiService mqttService, furchaContext db)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _lockerService = lockerService;
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _mqttService = mqttService;
            this.db = db;
        }

        private UserResult MapUserToUserResult(Domain.Entities.User user)
        {
            var roleName = user.Role.ToString();

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

        public List<UserResult> GetFilteredUsersByPagination(int adminId, int? filterByGroupId = null, int? filterByBranchId = null, int pageNumber = 1, int pageSize = 10)
        {
            var users = GetUsersForAdminBasedOnRole(adminId);

            var userIds = users.Select(x => x.Id).ToList();

            var filteredUsers = _userRepository.GetUsersByBranchGroupAndUserIds(userIds, filterByBranchId, filterByGroupId);

            var pagedUsers = filteredUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var userResults = pagedUsers.Select(user =>
            {
                var cards = _userRepository.GetUserCards(user.Id);
                var groups = _userRepository.GetUserGroupsByUserId(user.Id);
                var branches = _userRepository.GetUserBranchesByUserId(user.Id);

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

        public List<Domain.Entities.User> GetCompanyUsers(int companyId)
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

            var users = new List<Domain.Entities.User>();

            if (admin.Role == RoleEnum.LVL5_MasterAdmin)
            {
                users = GetUsersForLVL5Admin(admin.CompanyId);
            }
/*            else if (admin.Role == RoleEnum.LVL4_SuperAdmin)
            {
                users = GetUsersForLVL4Admin(admin);
            }
            else if (admin.Role != RoleEnum.user)
            {
                users = GetUsersForCommonAdmin(admin);
            }*/
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            return users.Select(user => MapUserToUserResult(user)).ToList();
        }

        public List<Domain.Entities.User> GetUsersForLVL5Admin(int companyId)
        {
            return _userRepository.GetCompanyUsers(companyId);
        }

        public List<Domain.Entities.User> GetUsersForLVL4Admin(Domain.Entities.Administrator admin)
        {
            var adminBranches = _userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();

            var users = _userRepository.GetUsersByBranches(distinctBranchIds);

            return users;
        }

        public List<Domain.Entities.User> GetUsersForCommonAdmin(Domain.Entities.Administrator admin)
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

            var groups = new List<Domain.Entities.UserGroup>();

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

        public async Task<UserResult> AddUser(UserCreateRequest newUser, int adminId)
        {
            if (newUser.IsPinRequired == true)
            {
                // TODO: Generate a 4-digit PIN unique within the branch
            }
            var companyUid = db.Administrators.Include(a => a.Company).FirstOrDefault(x => x.Id == adminId).Company.Id;
#warning Add columns in DB for ActiveTo and ActiveFrom. Determine user STATE based on that. Include this logic in GetAllUsersOfAdmin.

            var result = _userRepository.AddUser(newUser);

            var mqttRequest = new MqttBaseRequest<UserResult>
            {
                Command = (int)CommandTypes.CreateUserFromAdmin,
                ReceivedDate = DateTime.Now,
                Data = result
            };

            await _mqttService.PublishMqttCommands(mqttRequest, companyUid.ToString(), "1"); // Use claims or context for real companyId and branchId

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
