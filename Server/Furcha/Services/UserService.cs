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
using Mono.TextTemplating;
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
        private List<User> GetUsersByIdsBranchAndGroup(List<int> userIds, int? branchId, int? groupId, bool? isAdmin)
        {
            var query = Db.Users.Include(u => u.UserBranches).Include(u => u.Administrators)
                .Where(u => userIds.Contains(u.Id));

            if (branchId.HasValue)
            {
                query = query.Where(u => u.UserBranches.Any(ub => ub.BranchId == branchId.Value));
            }

            if (isAdmin == false)
            {
                query = query.Where(u => !u.Administrators.Any());
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

        public bool DeleteUsers(List<int> ids)
        {
            var admins = Db.Administrators.Where(a => ids.Contains(a.UserId ?? 0)).ToList();
            if (admins.Any())
                Db.Administrators.RemoveRange(admins);

            var users = Db.Users.Where(u => ids.Contains(u.Id)).ToList();
            if (users.Any())
                Db.Users.RemoveRange(users);

            Db.SaveChanges();
            return true;
        }

        public List<UserResult> GetFilteredUsersByPagination(int adminId, int? filterByGroupId = null, int? filterByBranchId = null, bool? isAdmin = null, int pageNumber = 1, int pageSize = 10)
        {
            var companyId = Db.Administrators.First(x => x.Id == adminId).CompanyId;

            var users = Db.Users.Where(u => u.CompanyId == companyId).ToList();

            var userIds = users.Select(x => x.Id).ToList();

            var filteredUsers = GetUsersByIdsBranchAndGroup(userIds, filterByBranchId, filterByGroupId, isAdmin);

            var pagedUsers = filteredUsers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var userResults = pagedUsers.Select(user =>
            {
                var cards = Db.Cards
                    .Where(c => c.UserId == user.Id)
                    .ToList();
                var groups = Db.UserGroups
                    .Where(ug => ug.Users.Any(u => u.Id == user.Id))
                    .ToList();
                var branches = Db.Branches
                    .Where(b => b.UserBranches.Any(ub => ub.UserId == user.Id))
                    .ToList();

                return new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    ActiveFrom = user.ActiveFrom,
                    ActiveTo = user.ActiveTo,
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
        .ToList();

            var branchFilterResults = branches.Select(branch => new BranchFilterResult
            {
                Id = branch.Id,
                Name = branch.Name
            }).ToList();

            return branchFilterResults;
        }

        public List<User> GetCompanyUsers(int companyId)
        {
            var users = Db.Users.Where(u => u.CompanyId == companyId).ToList();

            return users;
        }

        public List<UserResult> GetUsersForAdminBasedOnRole(int adminId)
        {
            var admin = Db.Administrators.First(a => a.Id == adminId);

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var users = new List<User>();

            if (admin.RoleId == (int)RoleEnum.LVL5_MasterAdmin)
            {
                users = GetUsersForLVL5Admin(admin.CompanyId);
            }           
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            return users.Select(user => MapUserToUserResult(user)).ToList();
        }

        public List<User> GetUsersForLVL5Admin(int companyId)
        {
            return Db.Users.Where(u => u.CompanyId == companyId).ToList();
        }

        public List<User> GetUsersForLVL4Admin(FurchaDAL.Models.Administrator admin)
        {
            var adminBranches = Db.AdminBranches
                        .Where(ab => ab.AdministratorId == admin.Id)
                        .Include(ab => ab.Branch)
                        .Select(ab => ab.Branch)
                        .ToList();

            var distinctBranchIds = adminBranches
                             .Select(ub => ub.Id)
                             .Distinct()
                             .ToList();

            var users = Db.UserBranches
                        .Where(ub => distinctBranchIds.Contains(ub.BranchId))
                        .Select(ub => ub.User)
                        .Distinct()
                        .ToList();
         
            return users;
        }

        public List<User> GetUsersForCommonAdmin(FurchaDAL.Models.Administrator admin)
        {
            var adminBranches = Db.Branches
                    .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == admin.Id))
                    .ToList();

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
             .ToList();

            return users;
        }

        public List<UserGroupResult>? GetUserGroupsForAdminBasedOnRole(int adminId)
        {
            var admin = Db.Administrators.Where(a => a.Id == adminId).FirstOrDefault();

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groups = new List<FurchaDAL.Models.UserGroup>();

            if (admin.RoleId == (long)RoleEnum.LVL5_MasterAdmin)
            {
                groups = Db.UserGroups
                     .Where(ug => ug.CompanyId == admin.CompanyId)
                     .ToList();
            }
            else if (admin.RoleId == (long)RoleEnum.LVL4_SuperAdmin)
            {
                var adminBranches = Db.AdminBranches
                     .Where(ab => ab.AdministratorId == admin.Id)
                     .Select(ab => ab.Branch)
                     .ToList();

                var distinctBranchIds = adminBranches
                                 .Select(ub => ub.Id)
                                 .Distinct()
                                 .ToList();

                groups = Db.UserGroups
                     .Where(ug => ug.UserGroupBranches
                         .Any(ugb => distinctBranchIds.Contains(ugb.BranchId)))
                     .Distinct()
                     .ToList();
            }
            else if (admin.RoleId != (long)RoleEnum.user)
            {
                var adminBranches = Db.Branches
                     .Include(b => b.AdminBranches)
                     .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == admin.Id))
                     .ToList();
               
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
                    .ToList();
            }
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groupResults = new List<UserGroupResult>();

            foreach (var group in groups)
            {
                var userGroupId = group.Id;
                var companyName = Db.Companies.Where(c => c.Id == group.CompanyId).First();
                var groupBranches = (from b in Db.Branches
                                     join ugb in Db.UserGroupBranches on b.Id equals ugb.BranchId
                                     where ugb.UserGroupId == userGroupId
                                     select b)
                   .Distinct()
                   .ToList();
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
        .Where(lg => Db.Lockers.Include(x => x.Brain)
            .Where(l => l.Brain.GroupId == lg.Id)
            .Join(Db.UserGroupLockers,
                  locker => locker.Id,
                  ugl => ugl.LockerId,
                  (locker, ugl) => ugl)
            .Any(ugl => ugl.UserGroupId == group.Id))
        .Distinct()
        .ToList(); 
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

        public UserResult AddUser(UserCreateRequest newUser, int adminId)
        {
            if (newUser.IsPinRequired == true)
            {
                // TODO: Generate a 4-digit PIN unique within the branch
            }
            var companyUid = Db.Administrators.Include(a => a.Company).FirstOrDefault(x => x.Id == adminId).Company.Id;

            var result = AddUserToDb(newUser, adminId);

            if (newUser.Id == 0)
            {
                var mqttRequest = new MqttBaseRequest<UserResult>
                {
                    Command = (int)CommandTypes.CreateUserFromAdmin,
                    ReceivedDate = DateTime.Now,
                    Data = result
                };

                _mqttService.PublishMqttCommands(mqttRequest, companyUid.ToString(), "1");
            }

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
                /*  using (var transactionScope = new TransactionScope(TransactionScopeOption.Required,
                      new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                      TransactionScopeAsyncFlowOption.Enabled))
                  {*/

                var user = Db.Users.FirstOrDefault(u => u.Id == newUser.Id);

                if (user == null)
                {
                    // Insert new user
                    user = new User
                    {
                        Name = newUser.Name,
                        Surname = newUser.Surname,
                        Email = newUser.Email,
                        Phone = newUser.Phone,
                        CreatedDate = DateOnly.FromDayNumber(1),
                        State = (int)state,
                        CompanyId = (int)companyId,
                        ActiveFrom = newUser.ActiveFrom,
                        ActiveTo = newUser.ActiveTo
                    };
                    Db.Users.Add(user);
                }
                else
                {
                    // Update existing user
                    user.Name = newUser.Name;
                    user.Surname = newUser.Surname;
                    user.Phone = newUser.Phone;
                    user.State = (int)state;
                    user.ActiveFrom = newUser.ActiveFrom;
                    user.ActiveTo = newUser.ActiveTo;
                }

                Db.SaveChanges();

                if (newUser.Cards != null)
                {
                    AddCardsByNumbers(newUser.Cards, user.Id);
                }
                if (newUser.LockerIds != null)
                {
                    AssignLockersToUser(newUser.LockerIds, user.Id);
                }

                AssignUserGroupsToUser(newUser.UserGroups, user.Id);

                //    transactionScope.Complete();

                return new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = RoleEnum.user.ToString(),
                    State = user.State.ToString()
                };
                /* }*/
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
                var user = Db.Users
                    .Include(u => u.UserGroups)
                    .FirstOrDefault(u => u.Id == userId);

                if (user == null)
                    throw new Exception($"User with Id {userId} not found");

                var existingGroupIds = user.UserGroups.Select(g => g.Id).ToHashSet();

                var newGroups = Db.UserGroups
                    .Where(g => groupIds.Contains(g.Id) && !existingGroupIds.Contains(g.Id))
                    .ToList();

                foreach (var group in newGroups)
                {
                    user.UserGroups.Add(group);
                }

                if (newGroups.Any())
                    Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }


        public void AssignLockersToUser(List<int> lockerIds, int userId)
        {
            var user = Db.Users
                .Include(u => u.Lockers)
                .Include(u => u.UserBranches)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException($"User with Id {userId} not found.");

            var existingLockerIds = user.Lockers.Select(l => l.Id).ToHashSet();

            var newLockers = Db.Lockers
                .Where(l => lockerIds.Contains(l.Id) && !existingLockerIds.Contains(l.Id))
                .Include(l => l.Brain)
                .ToList();

            foreach (var locker in newLockers)
            {
                user.Lockers.Add(locker);
            }

            var existingBranchIds = user.UserBranches.Select(ub => ub.BranchId).ToHashSet();

            var newBranchIds = newLockers
                .Where(l => l.Brain != null && l.Brain.BranchId.HasValue)
                .Select(l => l.Brain.BranchId.Value)
                .Distinct()
                .Where(branchId => !existingBranchIds.Contains(branchId))
                .ToList();

            foreach (var branchId in newBranchIds)
            {
                user.UserBranches.Add(new FurchaDAL.Models.UserBranch
                {
                    UserId = userId,
                    BranchId = branchId
                });
            }

            if (newLockers.Any() || newBranchIds.Any())
                Db.SaveChanges();
        }


        private List<FurchaDAL.Models.Card> AddCardsByNumbers(List<string> cardNumbers, int userId)
        {
            var result = new List<FurchaDAL.Models.Card>();

            try
            {
                var existingCardNumbers = Db.Cards
                    .Where(c => c.UserId == userId && cardNumbers.Contains(c.CardNumber))
                    .Select(c => c.CardNumber)
                    .ToHashSet();

                foreach (var cardNumber in cardNumbers)
                {
                    if (existingCardNumbers.Contains(cardNumber))
                        continue;

                    var card = new FurchaDAL.Models.Card
                    {
                        CardNumber = cardNumber,
                        UserId = userId
                    };

                    result.Add(card);
                    Db.Cards.Add(card);
                }

                if (result.Any())
                    Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }

            return result;
        }

        public void SetUserState(List<int> ids, int state)
        {
            var users = Db.Users.Where(a => ids.Contains(a.Id)).ToList();

            foreach (var u in users)
            {
                u.State = state;
            }

            Db.SaveChanges();
        }

        public void ChangeUsersGroup(List<int> ids, int groupId)
        {
            var users = Db.Users
                .Include(u => u.UserGroups)
                .Where(u => ids.Contains(u.Id))
                .ToList();

            var group = Db.UserGroups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
                throw new Exception("UserGroup not found");

            foreach (var u in users)
            {
                if (!u.UserGroups.Any(g => g.Id == groupId))
                    u.UserGroups.Add(group);
            }

            Db.SaveChanges();
        }

        public SingleUserResult GetUserById(int id)
        {
            var user = Db.Users.Include(x => x.UserBranches).ThenInclude(x => x.Branch).ThenInclude(x => x.BrainModules).ThenInclude(b => b.Lockers)
                .Include(u => u.Lockers).ThenInclude(l => l.Brain)
                .Include(x => x.Cards).Include(u => u.UserGroups).First(u => u.Id == id);

            return new SingleUserResult
            {
                Id = user.Id,
                IsPinRequired = false,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                ActiveFrom = user.ActiveFrom,
                ActiveTo = user.ActiveTo,
                State = user.State == 1 ? "Active" : "Suspended",
                UserGroups = user.UserGroups.Select(x => x.Id).ToList(),
                Cards = user.Cards.Select(x => x.CardNumber).ToList(),
                Branches = user.UserBranches.Select(b => new BranchResult
                {
                    Id = b.BranchId,
                    Name = b.Branch.Name,
                    Lockers = user.Lockers.Where(l => l.Brain.BranchId == b.BranchId).Select(x => x.Id).ToList(),
                }).ToList(),
            };
        }

    }
}
