using Domain.Entities;
using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;
using FurchaDAL.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Transactions;
using User = FurchaDAL.Models.User;


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
        private readonly furchaContext Db;

        public UserService(UserRepository userRepository, AdminRepository adminRepository, LockerService lockerService, LockerRepository lockerRepository, BranchRepository branchRepository, IMqttApiService mqttService, furchaContext db)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _lockerService = lockerService;
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _mqttService = mqttService;
            Db = db;
        }
        private List<User> GetUsersByIdsBranchAndGroup(List<int> userIds, int? branchId, int? groupId)
        {
            var query = Db.Users
                .Where(u => userIds.Contains(u.Id));

            if (branchId.HasValue)
            {
                query = query.Where(u => u.UserBranches.Any(ub => ub.BranchId == branchId.Value));
            }

            if (groupId.HasValue)
            {
                query = query.Where(u => u.UserGroups.Any(ug => ug.Id == groupId.Value));
            }

            return query.ToList();
        }

        private UserResult MapUserToUserResult(User user)
        {
            var roles = Db.Roles.ToList();
            var userRole = Db.Administrators.Where(a => a.UserId == user.Id).FirstOrDefault();
            string roleName;

            if (userRole != null)
            {
                 roleName = roles.Where(r => r.Id == userRole.RoleId).First().Name;
            }
            else
            {
                roleName = roles.Where(r => r.Id == (long)RoleEnum.user).First().Name;
            }

            var cards = Db.Cards
                .Where(c => c.UserId == user.Id)
                .ToList(); // _userRepository.GetUserCards(user.Id);

            var userWithGroups = Db.Users
                 .Include(u => u.UserGroups)
                 .FirstOrDefault(u => u.Id == user.Id);

            var groups = userWithGroups?.UserGroups.ToList();
            // _userRepository.GetUserGroupsByUserId(user.Id);
            //  var branches = _userRepository.GetUserBranchesByUserId(user.Id);
            var branches = Db.UserBranches
                .Where(ub => ub.UserId == user.Id)
                .Select(ub => ub.Branch)
                .ToList();

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

            var filteredUsers = GetUsersByIdsBranchAndGroup(userIds, filterByBranchId, filterByGroupId); // _userRepository.GetUsersByBranchGroupAndUserIds(userIds, filterByBranchId, filterByGroupId);

            var pagedUsers = filteredUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var userResults = pagedUsers.Select(user =>
            {
                var cards = Db.Cards
                    .Where(c => c.UserId == user.Id)
                    .ToList();// _userRepository.GetUserCards(user.Id);
                var groups = Db.UserGroups
                    .Where(ug => ug.Users.Any(u => u.Id == user.Id))
                    .ToList(); // _userRepository.GetUserGroupsByUserId(user.Id);
                var branches = Db.Branches
                    .Where(b => b.UserBranches.Any(ub => ub.UserId == user.Id))
                    .ToList(); // _userRepository.GetUserBranchesByUserId(user.Id);

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
            var branches = Db.Branches
        .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == adminId))
        .ToList(); //  _userRepository.GetAllBranchesOfAdmin(AdminId);

            var branchFilterResults = branches.Select(branch => new BranchFilterResult
            {
                Id = branch.Id,
                Name = branch.Name
            }).ToList();

            return branchFilterResults;
        }

        public List<User> GetCompanyUsers(int companyId)
        {
            var users = Db.Users.Where(u => u.CompanyId == companyId).ToList(); //_userRepository.GetCompanyUsers(companyId);

            return users;
        }

        public List<UserResult> GetUsersForAdminBasedOnRole(int adminId)
        {
            var admin = Db.Administrators.First(a => a.Id == adminId); // _userRepository.GetAdminById(AdminId);
            
            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var users = new List<User>();

            if (admin.RoleId == (int)RoleEnum.LVL5_MasterAdmin)
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

        public List<User> GetUsersForLVL5Admin(int companyId)
        {
            return Db.Users.Where(u => u.CompanyId == companyId).ToList();//_userRepository.GetCompanyUsers(companyId);
        }

        public List<User> GetUsersForLVL4Admin(FurchaDAL.Models.Administrator admin)
        {
            var adminBranches =  Db.AdminBranches
                        .Where(ab => ab.AdministratorId == admin.Id)
                        .Include(ab => ab.Branch)
                        .Select(ab => ab.Branch)
                        .ToList();//_userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();

            var users = Db.UserBranches
                        .Where(ub => distinctBranchIds.Contains(ub.BranchId))
                        .Select(ub => ub.User)
                        .Distinct()
                        .ToList();
            //_userRepository.GetUsersByBranches(distinctBranchIds);

            return users;
        }

        public List<User> GetUsersForCommonAdmin(FurchaDAL.Models.Administrator admin)
        {
            var adminBranches = Db.Branches
                    .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == admin.Id))
                    .ToList();// _userRepository.GetAdminBranchesByAdminId(admin.Id);

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();
            if (distinctBranchIds.Count > 1)
            {
                throw new BaseException(ErrorCodeEnum.AdminHasMoreBranchesThanPermitted);
            }
            var users = Db.Users
             .Where(u => u.UserBranches.Any(ub => ub.BranchId == distinctBranchIds[0]))
             .ToList(); //_userRepository.GetAllUsersOfBranch(distinctBranchIds[0]);

            return users;
        }

        public List<UserGroupResult>? GetUserGroupsForAdminBasedOnRole(int adminId)
        {
            var admin = Db.Administrators.Where(a => a.Id == adminId).FirstOrDefault();// _userRepository.GetAdminById(AdminId);

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groups = new List<FurchaDAL.Models.UserGroup>();

            if (admin.RoleId == (long)RoleEnum.LVL5_MasterAdmin)
            {
                groups = Db.UserGroups
                     .Where(ug => ug.CompanyId == admin.CompanyId)
                     .ToList();  //_userRepository.GetUserGroupsByCompanyId(admin.CompanyId);
            }
            else if (admin.RoleId == (long)RoleEnum.LVL4_SuperAdmin)
            {
                var adminBranches = Db.AdminBranches
                     .Where(ab => ab.AdministratorId == admin.Id)
                     .Select(ab => ab.Branch)
                     .ToList(); //_userRepository.GetAdminBranchesByAdminId(admin.Id);

                var distinctBranchIds = adminBranches
                                 .Select(ub => ub.Id)
                                 .Distinct()
                                 .ToList();

                groups = Db.UserGroups
                     .Where(ug => ug.UserGroupBranches
                         .Any(ugb => distinctBranchIds.Contains(ugb.BranchId)))
                     .Distinct()
                     .ToList();  //_userRepository.GetUserGroupsByBranchIds(distinctBranchIds);
            }
            else if (admin.RoleId != (long)RoleEnum.user) //if a role is added this condition may change
            {
               var adminBranches = Db.Branches
                    .Include(b => b.AdminBranches)
                    .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == admin.Id))
                    .ToList();
                //_userRepository.GetAdminBranchesByAdminId(admin.Id);

                var distinctBranchIds = adminBranches
                                 .Select(ub => ub.Id)
                                 .Distinct()
                                 .ToList();

                if (distinctBranchIds.Count > 1)
                {
                    throw new BaseException(ErrorCodeEnum.AdminHasMoreBranchesThanPermitted);
                }

                groups = Db.UserGroups
                    .Where(ug => ug.UserGroupBranches.Any(ugb => distinctBranchIds.Contains(ugb.BranchId)))
                    .Distinct()
                    .ToList(); //_userRepository.GetUserGroupsByBranchIds(distinctBranchIds);
            }
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groupResults = new List<UserGroupResult>();

            foreach (var group in groups)
            {
                var userGroupId = group.Id;
                var companyName = Db.Companies.Where(c => c.Id == group.CompanyId).First(); // _branchRepository.GetCompanyById(group.CompanyId).Name;
                var groupBranches = (from b in Db.Branches
                                     join ugb in Db.UserGroupBranches on b.Id equals ugb.BranchId
                                     where ugb.UserGroupId == userGroupId
                                     select b)
                   .Distinct()
                   .ToList();
                // _branchRepository.GetBranchesOfUserGroup(group.Id);
                var permittedLockers = _lockerService.GetPermittedLockersOfUserGroup(ugId: group.Id);
                var permittedLockerResults = new List<PermittedLockerResult>();
                foreach (var permittedLocker in permittedLockers)
                {
                    var permittedLockerResult = new PermittedLockerResult()
                    {
                        LockerId = permittedLocker.Id,
                        LockerNumber = (int)permittedLocker.Number
                    };

                    permittedLockerResults.Add(permittedLockerResult);
                }

                var lockerGroups = Db.LockerGroups
        .Where(lg => Db.Lockers
            .Where(l => l.GroupId == lg.Id)
            .Join(Db.UserGroupLockers,
                  locker => locker.Id,
                  ugl => ugl.LockerId,
                  (locker, ugl) => ugl)
            .Any(ugl => ugl.UserGroupId == group.Id))
        .Distinct()
        .ToList();  //_lockerRepository.GetLockerGroupsByUserGroup(group.Id);
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
            var companyUid = Db.Administrators.Include(a => a.Company).FirstOrDefault(x => x.Id == adminId).Company.Id;
#warning Add columns in DB for ActiveTo and ActiveFrom. Determine user STATE based on that. Include this logic in GetAllUsersOfAdmin.

            var result = AddUserToDb(newUser, adminId);

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
            var group = AddUserGroupToDb(userGroupRequest);

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

        private FurchaDAL.Models.UserGroup AddUserGroupToDb(UserGroupRequest userGroupRequest)
        {
            var companyId = Db.Administrators.FirstOrDefault(a => a.Id == userGroupRequest.AdminId).CompanyId;

            if (companyId == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "Company not found for admin.");
            }

            var group = new FurchaDAL.Models.UserGroup
            {
                State = (int)StateEnum.active,
                CompanyId = (int)companyId,
                Name = userGroupRequest.Name,
                Description = string.Empty
            };

            Db.Add(group);
            Db.SaveChanges();

            return group;
        }


        private UserResult AddUserToDb(UserCreateRequest newUser, int adminId)
        {
            var companyId = Db.Administrators.FirstOrDefault(a => a.Id == adminId).CompanyId; 
            var state = StateEnum.active;

            if (companyId == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            if (newUser.ActiveFrom != null && newUser.ActiveFrom > DateTime.Now)
            {
                state = StateEnum.active;
            }
            else if (newUser.ActiveTo < DateTime.Now)
            {
                state = StateEnum.expanded;
            }

            try
            {
                using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = new User
                    {
                        Name = newUser.Name,
                        Surname = newUser.Surname,
                        Email = newUser.Email,
                        Phone = newUser.Phone,
                        CreatedDate = DateOnly.FromDayNumber(1),
                        State = (int)state,
                        CompanyId = (int)companyId,
                    };

                    Db.Add(user);
                    Db.SaveChanges();

                    AddCardsByNumbers(newUser.Cards, user.Id);
                    AssignLockersToUser(newUser.LockerIds, user.Id);
                    AssignUserGroupsToUser(newUser.UserGroups, user.Id);

                    transactionScope.Complete();

                    return new UserResult
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Role = RoleEnum.user.ToString(),
                        State = user.State.ToString()
                    };
                }
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }

        private void AssignUserGroupsToUser(List<int> groupIds, int userId)
        {
            try
            {
                foreach (var id in groupIds)
                {
                    var userUserGroup = new User_UserGroup
                    {
                        UserId = userId,
                        UserGroupId = id
                    };

                    Db.Add(userUserGroup);
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }

        private void AssignLockersToUser(List<int> lockerIds, int userId)
        {
            try
            {
                foreach (var id in lockerIds)
                {
                    var userLocker = new UserLocker
                    {
                        UserId = userId,
                        LockerId = id
                    };

                    Db.Add(userLocker);
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }

        private List<FurchaDAL.Models.Card> AddCardsByNumbers(List<string> cardNumbers, int userId)
        {
            var result = new List<FurchaDAL.Models.Card>();

            try
            {
                foreach (var cardNumber in cardNumbers)
                {
                    var card = new FurchaDAL.Models.Card
                    {
                        CardNumber = cardNumber,
                        UserId = userId
                    };

                    result.Add(card);
                    Db.Add(card);
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }

            return result;
        }


    }
}
