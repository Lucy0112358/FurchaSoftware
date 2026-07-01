using Domain.Enums;
using Domain.Exceptionss;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaBLL.Constants;
using FurchaBLL.Interfaces;
using FurchaBLL.MqttModels.Subscribe;
using FurchaBLL.Services;
using FurchaDAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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

        private readonly SyncTaskService _syncTaskService;
        public UserService(UserRepository userRepository, AdminRepository adminRepository, LockerService lockerService, LockerRepository lockerRepository, BranchRepository branchRepository, IMqttApiService mqttService, furchaContext db, SyncTaskService syncTaskService)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _lockerService = lockerService;
            _lockerRepository = lockerRepository;
            _branchRepository = branchRepository;
            _mqttService = mqttService;
            Db = db;
            _syncTaskService = syncTaskService;
        }

        private async Task WithTransactionAsync(Func<Task> work)
        {
            var strategy = Db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using (var tx = await Db.Database.BeginTransactionAsync())
                {
                    await work();
                    await tx.CommitAsync();
                }
            });
        }

        private List<User> GetUsersByIdsBranchAndGroup(
       List<int> userIds,
       int? branchId,
       int? groupId,
       bool? isAdmin,
       string? name)
        {
            var query = Db.Users
                .Include(u => u.UserBranches)
                .Include(u => u.Administrators)
                .Include(u => u.UserGroups)
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

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();

                query = query.Where(u =>
                    (
                        u.Name != null &&
                        EF.Functions.Like(u.Name, "%" + name + "%")
                    ) ||
                    (
                        u.Surname != null &&
                        EF.Functions.Like(u.Surname, "%" + name + "%")
                    ) ||
                    (
                        (u.Name + " " + u.Surname).Trim() != "" &&
                        EF.Functions.Like((u.Name + " " + u.Surname), "%" + name + "%")
                    )
                );
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
                State = user.State,
                Cards = cards.Select(card => new CardResult { Id = card.Id, CardNumber = card.CardNumber }).ToList(),
                UserGroups = groups.Select(group => new UserGroupResult { Id = group.Id, Name = group.Name }).ToList(),
                Branches = branches.Select(branch => new BranchResult { Id = branch.Id, Name = branch.Name }).ToList()
            };
        }

        public async Task<bool> DeleteUserGroups(List<int> ids)
        {
            await WithTransactionAsync(async () =>
            {
                foreach (var id in ids)
                {
                    var group = Db.UserGroups
                        .Include(g => g.UserGroupBranches)
                        .Include(g => g.UserGroupLockers)
                        .Include(g => g.Users) // many-to-many
                        .FirstOrDefault(g => g.Id == id);

                    if (group == null)
                        continue;

                    // 1. Remove one-to-many: UserGroupBranches
                    if (group.UserGroupBranches.Any())
                        Db.UserGroupBranches.RemoveRange(group.UserGroupBranches);

                    // 2. Remove one-to-many: UserGroupLockers
                    if (group.UserGroupLockers.Any())
                    {
                        var lockerIds = group.UserGroupLockers.Select(ugl => ugl.LockerId).ToList();

                        Db.UserGroupLockers.RemoveRange(group.UserGroupLockers);

                        await NotifyUserGroupLockerAssignment(group, lockerIds, "remove");
                    }

                    // 3. Clear many-to-many: UserGroup ↔ Users
                    if (group.Users.Any())
                        group.Users.Clear(); // removes rows from the join table

                    // 4. Finally delete the group itself
                    Db.UserGroups.Remove(group);
                }

                Db.SaveChanges();
            });

            return true;
        }


        public async Task<bool> DeleteUsers(List<int> ids)
        {
            await WithTransactionAsync(async () =>
            {
                foreach (var id in ids)
                {
                    var user = Db.Users
                        .Include(u => u.Administrators)
                        .Include(u => u.Cards)
                        .Include(u => u.UserBranches)
                        .Include(u => u.UserGroups)   // many-to-many
                        .Include(u => u.Lockers)      // many-to-many
                        .FirstOrDefault(u => u.Id == id);

                    if (user == null)
                        continue;

                    // 1. Remove related administrators
                    if (user.Administrators.Any())
                        Db.Administrators.RemoveRange(user.Administrators);

                    // 2. Remove one-to-many: UserBranches
                    if (user.UserBranches.Any())
                        Db.UserBranches.RemoveRange(user.UserBranches);

                    // 3. Remove one-to-many: Cards
                    if (user.Cards.Any())
                        Db.Cards.RemoveRange(user.Cards);

                    // 4. Clear many-to-many: User ↔ UserGroups
                    user.UserGroups.Clear();

                    // 5. Clear many-to-many: User ↔ Lockers

                    if (user.Lockers.Any())
                    {
                        await NotifyUserLockerAssignment(user, user.Lockers.Select(l => l.Id).ToList(), "remove");
                    }

                    user.Lockers.Clear();

                    // 6. Finally delete the user
                    Db.Users.Remove(user);
                }

                Db.SaveChanges();
            });

            return true;
        }

        public List<UserResult> GetFilteredUsersByPagination(int adminId, int? filterByGroupId = null, int? filterByBranchId = null, bool? isAdmin = null, string? name = null, int pageNumber = 1, int pageSize = 10)
        {
            var companyId = Db.Administrators.First(x => x.Id == adminId).CompanyId;
            SyncExpiredActiveUsersToInactive(companyId);

            var users = Db.Users.Where(u => u.CompanyId == companyId).ToList();

            var userIds = users.Select(x => x.Id).ToList();

            var filteredUsers = GetUsersByIdsBranchAndGroup(userIds, filterByBranchId, filterByGroupId, isAdmin, name);

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
                var userAdmin = Db.Administrators.FirstOrDefault(a => a.UserId == user.Id);

                var roleName = userAdmin != null
                    ? Db.Roles.Where(r => r.Id == userAdmin.RoleId).Select(r => r.Name).FirstOrDefault()
                    : Db.Roles.Where(r => r.Id == (long)RoleEnum.user).Select(r => r.Name).FirstOrDefault();

                roleName ??= RoleEnum.user.ToString();

                return new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    ActiveFrom = user.ActiveFrom,
                    ActiveTo = user.ActiveTo,
                    Surname = user.Surname,
                    Role = roleName,
                    State = user.State,
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

        private void SyncExpiredActiveUsersToInactive(int companyId)
        {
            var today = DateTime.UtcNow.Date;

            Db.Users
              .Where(u =>
                  u.State == (int)StateEnum.active &&
                  u.ActiveTo.HasValue &&
                  u.ActiveTo.Value < DateOnly.FromDateTime(today))
              .ExecuteUpdate(s =>
                  s.SetProperty(u => u.State, (int)StateEnum.expanded));
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

        public List<UserGroupResult>? GetUserGroupsForAdminBasedOnRole(int adminId, int? branchId = null)
        {
            var admin = Db.Administrators.FirstOrDefault(a => a.Id == adminId);

            if (admin == null)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            var groups = new List<FurchaDAL.Models.UserGroup>();

            // --------------------
            // ROLE-BASED FETCHING
            // --------------------
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
                     .Select(ab => ab.BranchId)
                     .Distinct()
                     .ToList();

                groups = Db.UserGroups
                     .Where(ug => ug.UserGroupBranches
                         .Any(ugb => adminBranches.Contains(ugb.BranchId)))
                     .Distinct()
                     .ToList();
            }
            else if (admin.RoleId != (long)RoleEnum.user)
            {
                var adminBranches = Db.Branches
                     .Include(b => b.AdminBranches)
                     .Where(b => b.AdminBranches.Any(ab => ab.AdministratorId == admin.Id))
                     .Select(b => b.Id)
                     .Distinct()
                     .ToList();

                if (adminBranches.Count > 1)
                {
                    throw new BaseException(ErrorCodeEnum.AdminHasMoreBranchesThanPermitted);
                }

                groups = Db.UserGroups
                    .Where(ug => ug.UserGroupBranches.Any(ugb => adminBranches.Contains(ugb.BranchId)))
                    .Distinct()
                    .ToList();
            }
            else
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);
            }

            // ------------------------------------
            // OPTIONAL BRANCH FILTERING ADDED HERE
            // ------------------------------------
            if (branchId.HasValue)
            {
                groups = groups
                    .Where(g => Db.UserGroupBranches
                        .Any(ugb => ugb.UserGroupId == g.Id && ugb.BranchId == branchId.Value))
                    .ToList();
            }

            // ----------------------------
            // BUILD RESULT OUTPUT
            // ----------------------------
            var groupResults = new List<UserGroupResult>();

            foreach (var group in groups)
            {
                var userGroupId = group.Id;

                var groupBranches = (from b in Db.Branches
                                     join ugb in Db.UserGroupBranches on b.Id equals ugb.BranchId
                                     where ugb.UserGroupId == userGroupId
                                     select b)
                    .Distinct()
                    .ToList();

                var permittedLockers = _lockerService.GetPermittedLockersOfUserGroup(ugId: group.Id);

                var permittedLockerResults = permittedLockers
                    .Select(pl => new PermittedLockerResult
                    {
                        LockerId = pl.Id,
                        LockerNumber = (int)pl.Number
                    })
                    .ToList();

                var permittedLockerIds = Db.UserGroupLockers
                    .Where(ugl => ugl.UserGroupId == group.Id)
                    .Select(ugl => ugl.LockerId)
                    .ToList();

                var lockerGroups = Db.LockerGroups
                    .Include(lg => lg.BrainModules)
                        .ThenInclude(bm => bm.Lockers)
                    .Where(lg =>
                        lg.BrainModules
                            .SelectMany(bm => bm.Lockers)
                            .Any(l => permittedLockerIds.Contains(l.Id)))
                    .Distinct()
                    .ToList();

                var lockerGroupResults = lockerGroups
                    .Select(lg => new LockerGroupResult
                    {
                        LockerGroupName = lg.Name,
                        LockersFromGroup = lg.BrainModules
                            .SelectMany(bm => bm.Lockers)
                            .Where(l => permittedLockerIds.Contains(l.Id))
                            .Select(l => new PermittedLockerResult
                            {
                                LockerId = l.Id,
                                LockerNumber = (int)l.Number
                            })
                            .ToList()
                    })
                    .ToList();


                groupResults.Add(new UserGroupResult
                {
                    Id = group.Id,
                    Name = group.Name,
                    PermittedLockers = lockerGroupResults,
                    BranchNames = groupBranches.Select(b => b.Name).ToList(),
                    State = (int)group.State,
                    UserCount = Db.Users.Count(u => u.UserGroups.Any(ug => ug.Id == group.Id))
                });
            }

            return groupResults;
        }


        public List<UserResult> SearchUsersOfAdmin(string? name, int adminId)
        {
            var users = GetUsersForAdminBasedOnRole(adminId);

            if (string.IsNullOrWhiteSpace(name))
                return users; // return all users

            name = name.Trim();

            var filteredUsers = users
                .Where(u =>
                    (!string.IsNullOrEmpty(u.Name) &&
                     u.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ||

                    (!string.IsNullOrEmpty(u.Surname) &&
                     u.Surname.Contains(name, StringComparison.OrdinalIgnoreCase))
                )
                .ToList();

            return filteredUsers;
        }

        public async Task<UserResult> AddUser(UserCreateRequest newUser, int adminId)
        {
            if (newUser.IsPinRequired == true)
            {
                // TODO: Generate a 4-digit PIN unique within the branch
            }

            UserResult result = null!;

            // Data change + the sync task(s) enqueued inside AddUserToDb commit atomically.
            await WithTransactionAsync(async () =>
            {
                result = await AddUserToDb(newUser, adminId);
            });

            return result;
        }

        public async Task<UserGroupResult> AddUserGroup(UserGroupRequest request)
        {

            var existsWithSameName = Db.UserGroups
                .Any(ug => ug.Name == request.Name);

            if (existsWithSameName)
            {
                throw new BaseException(ErrorCodeEnum.UserGroupNameExists, "A group with this name already exists.");
            }


            UserGroupResult result = null!;

            await WithTransactionAsync(async () =>
            {
                var group = AddUserGroupToDb(request);

                // ---------------------------------------
                // 2. Insert LOCKERS first
                // ---------------------------------------
                List<int> insertedLockerIds = new();

                if (request.LockerIds?.Any() == true)
                {
                    var validLockerIds = Db.Lockers.Select(l => l.Id).ToHashSet();

                    insertedLockerIds = request.LockerIds
                        .Where(id => validLockerIds.Contains(id))
                        .ToList();

                    var lockerLinks = insertedLockerIds
                        .Select(lockerId => new UserGroupLocker
                        {
                            UserGroupId = group.Id,
                            LockerId = lockerId
                        })
                        .ToList();

                    Db.UserGroupLockers.AddRange(lockerLinks);
                    Db.SaveChanges();

                    await NotifyUserGroupLockerAssignment(group, insertedLockerIds, "add");
                }

                // ---------------------------------------
                // 3. Derive BRANCHES from lockers' Brain.BranchId
                // ---------------------------------------
                var branchIds = Db.Lockers
                    .Where(l => insertedLockerIds.Contains(l.Id))
                    .Select(l => l.Brain.BranchId)
                    .Where(b => b.HasValue)
                    .Select(b => b.Value)
                    .Distinct()
                    .ToList();

                if (branchIds.Any())
                {
                    var branchLinks = branchIds
                        .Select(branchId => new UserGroupBranch
                        {
                            UserGroupId = group.Id,
                            BranchId = branchId
                        })
                        .ToList();

                    Db.UserGroupBranches.AddRange(branchLinks);
                    Db.SaveChanges();
                }

                // ---------------------------------------
                // 4. Build result: Branch names
                // ---------------------------------------
                var branchNames = Db.Branches
                    .Where(b => branchIds.Contains(b.Id))
                    .Select(b => b.Name)
                    .ToList();

                // ---------------------------------------
                // 5. Build result: Locker groups
                // ---------------------------------------
                var lockerGroups = Db.LockerGroups
                    .Include(lg => lg.BrainModules)
                        .ThenInclude(bm => bm.Lockers)
                    .Where(lg =>
                        lg.BrainModules
                            .SelectMany(b => b.Lockers)
                            .Any(l => insertedLockerIds.Contains(l.Id)))
                    .ToList();

                var lockerGroupResults = lockerGroups
                    .Select(lg => new LockerGroupResult
                    {
                        LockerGroupName = lg.Name,
                        LockersFromGroup = lg.BrainModules
                            .SelectMany(bm => bm.Lockers)
                            .Where(l => insertedLockerIds.Contains(l.Id))
                            .Select(l => new PermittedLockerResult
                            {
                                LockerId = l.Id,
                                LockerNumber = (int)l.Number
                            })
                            .ToList()
                    })
                    .ToList();

                // ---------------------------------------
                // FINAL RESULT
                // ---------------------------------------
                result = new UserGroupResult
                {
                    Id = group.Id,
                    Name = group.Name,
                    State = (int)group.State,
                    BranchNames = branchNames,
                    PermittedLockers = lockerGroupResults
                };
            });

            return result;
        }

        private async Task NotifyUserLockerAssignment(User user, List<int> lockerIds, string action)
        {
            if (lockerIds == null || !lockerIds.Any())
                return;

            var lockers = Db.Lockers
                .Include(l => l.Brain)
                    .ThenInclude(b => b.Company)
                .Where(l => lockerIds.Contains(l.Id))
                .ToList();

            var brainGroups = lockers
                .Where(l => l.Brain?.Company?.AccountUid != null)
                .GroupBy(l => l.Brain);

            foreach (var brainGroup in brainGroups)
            {
                var brain = brainGroup.Key;

                var lockerExternalIds = brainGroup
                    .Select(l => l.ExternalId.GetValueOrDefault())
                    .ToArray();

                var data = new AssigningUserToLockerRequest
                {
                    ActiveFrom = null,
                    ActiveTo = null,
                    Cards = user?.Cards?.Select(c => c.CardNumber)?.ToArray() ?? [],
                    Doors = [], // not implemented yet
                    ExternalId = user.Id,
                    Lastname = user.Surname,
                    Lockers = lockerExternalIds,
                    Name = user.Name,
                    PersonalId = null, // null for now, not implemented yet
                    Pin = null, // null for now, not implemented yet
                    Rules = [], // not implemented yet
                    // Brain contract: 1 = active, 0 = suspended (not the cloud StateEnum scale).
                    State = action == "remove" ? 0 : (user.State == (int)StateEnum.active ? 1 : 0),
                    UserGroup = user.UserGroups?.FirstOrDefault()?.Name
                };

                var syncTask = SyncTaskService.BuildSyncTask(
                    SyncTaskStatus.Pending,
                    brain.Company.AccountUid.GetValueOrDefault(),
                    brain.BrainUid,
                    CommandTypes.AssigningUserToLocker,
                    user.Id,
                    "User",
                    action == "remove" ? SyncTaskOperationType.Delete : SyncTaskOperationType.Upsert,
                    data
                );

                await _syncTaskService.EnqueueAsync(syncTask);
            }
        }

        private async Task NotifyUserGroupLockerAssignment(UserGroup group, List<int> lockerIds, string action)
        {
            if (lockerIds == null || !lockerIds.Any())
                return;

            var lockers = Db.Lockers
                .Include(l => l.Brain)
                    .ThenInclude(b => b.Company)
                .Where(l => lockerIds.Contains(l.Id))
                .ToList();

            var brainGroups = lockers
                .Where(l => l.Brain?.Company?.AccountUid != null)
                .GroupBy(l => l.Brain);

            foreach (var brainGroup in brainGroups)
            {
                var brain = brainGroup.Key;

                var lockerExternalIds = brainGroup
                    .Select(l => (long)l.ExternalId.GetValueOrDefault())
                    .ToArray();

                var data = new AssigningUserGroupToLockerRequest
                {
                    GroupExternalId = group.Id,
                    Lockers = lockerExternalIds,
                    Doors = [],
                    ActiveFrom = null,
                    ActiveTo = null,
                    Entity = "UserGroup",
                    MemberExternalIds = group.Users?.Select(u => u.Id)?.ToArray() ?? [],
                    Name = group.Name,
                    State = action == "remove" ? 0 : 1
                };

                var syncTask = SyncTaskService.BuildSyncTask(
                    SyncTaskStatus.Pending,
                    brain.Company.AccountUid.GetValueOrDefault(),
                    brain.BrainUid,
                    CommandTypes.AssigningUserGroupToLocker,
                    group.Id,
                    "UserGroup",
                    action == "remove" ? SyncTaskOperationType.Delete : SyncTaskOperationType.Upsert,
                    data
                );

                await _syncTaskService.EnqueueAsync(syncTask);
            }
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

        private async Task<UserResult> AddUserToDb(UserCreateRequest newUser, int adminId)
        {
            var companyId = Db.Administrators
                .Where(a => a.Id == adminId)
                .Select(a => a.CompanyId)
                .FirstOrDefault();

            if (companyId == null)
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry);

            // Normalize email
            var email = newUser.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "Email is required.");

            // Dates
            DateTime? activeFrom = null;
            if (newUser.ActiveFrom.HasValue)
                activeFrom = DateTime.SpecifyKind(newUser.ActiveFrom.Value.Date, DateTimeKind.Unspecified);

            DateTime? activeTo = null;
            if (newUser.ActiveTo.HasValue)
                activeTo = DateTime.SpecifyKind(newUser.ActiveTo.Value.Date, DateTimeKind.Unspecified);

            var today = DateTime.UtcNow.Date;

            try
            {
                var user = Db.Users.FirstOrDefault(u => u.Id == newUser.Id);

                // 🔐 STATE RESOLUTION (FIX)
                StateEnum newState;

                if (user != null && user.State == (int)StateEnum.suspended)
                {
                    // Admin suspension always wins
                    newState = StateEnum.suspended;
                }
                else
                {
                    newState = StateEnum.active;

                    if (activeTo.HasValue && activeTo.Value.Date < today)
                        newState = StateEnum.expanded;
                }

                if (user == null)
                {
                    // ADD
                    var emailExists = Db.Users.Any(u => u.CompanyId == companyId && u.Email == email);
                    if (emailExists)
                        throw new BaseException(ErrorCodeEnum.EmailAlreadyExists,
                            "A user with this email already exists in your company.");

                    user = new User
                    {
                        Name = newUser.Name,
                        Surname = newUser.Surname,
                        Email = email,
                        Phone = newUser.Phone,
                        CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        State = (int)newState,
                        CompanyId = companyId,
                        ActiveFrom = activeFrom.HasValue ? DateOnly.FromDateTime(activeFrom.Value) : null,
                        ActiveTo = activeTo.HasValue ? DateOnly.FromDateTime(activeTo.Value) : null
                    };

                    Db.Users.Add(user);
                }
                else
                {
                    // EDIT
                    if (user.CompanyId != companyId)
                        throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "Access denied.");

                    var emailExists = Db.Users.Any(u =>
                        u.CompanyId == companyId &&
                        u.Email == email &&
                        u.Id != user.Id);

                    if (emailExists)
                        throw new BaseException(ErrorCodeEnum.EmailAlreadyExists,
                            "A user with this email already exists in your company.");

                    user.Name = newUser.Name;
                    user.Surname = newUser.Surname;
                    user.Email = email;
                    user.Phone = newUser.Phone;
                    user.State = (int)newState;
                    user.ActiveFrom = activeFrom.HasValue ? DateOnly.FromDateTime(activeFrom.Value) : null;
                    user.ActiveTo = activeTo.HasValue ? DateOnly.FromDateTime(activeTo.Value) : null;
                }

                Db.SaveChanges();

                if (newUser.Cards != null)
                    AddCardsByNumbers(newUser.Cards, user.Id);

                if (newUser.UserGroups != null)
                    AssignUserGroupsToUser(newUser.UserGroups, user.Id);

                // Lockers last: this enqueues the sync task, so cards + groups must already be
                // persisted for the payload to reflect the full user state.
                if (newUser.LockerIds != null)
                    await AssignLockersToUser(newUser.LockerIds, user.Id);

                return new UserResult
                {
                    Id = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Role = RoleEnum.user.ToString(),
                    State = user.State,
                    ActiveFrom = user.ActiveFrom,
                    ActiveTo = user.ActiveTo,
                    Phone = user.Phone
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx &&
                    (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                {
                    throw new BaseException(
                        ErrorCodeEnum.EmailAlreadyExists,
                        "A user with this email already exists in your company."
                    );
                }

                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }


        private DateTime? Normalize(DateTime? dt)
        {
            if (!dt.HasValue)
                return null;

            return DateTime.SpecifyKind(dt.Value, DateTimeKind.Unspecified);
        }

        private DateTime? Normalize(DateOnly? dt)
        {
            if (!dt.HasValue)
                return null;

            return DateTime.SpecifyKind(dt.Value.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
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

                var existingGroupIds = user.UserGroups.Select(g => g.Id).ToList();

                var groupsToRemove = user.UserGroups
                    .Where(g => !groupIds.Contains(g.Id))
                    .ToList();

                foreach (var group in groupsToRemove)
                {
                    user.UserGroups.Remove(group);
                }

                var groupsToAdd = Db.UserGroups
                    .Where(g => groupIds.Contains(g.Id) && !existingGroupIds.Contains(g.Id))
                    .ToList();

                foreach (var group in groupsToAdd)
                {
                    user.UserGroups.Add(group);
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }
        }

        public async Task AssignLockersToUser(List<int> lockerIds, int userId)
        {
            var user = Db.Users
                .Include(u => u.Lockers)
                .Include(u => u.UserBranches)
                .Include(u => u.Cards)
                .Include(u => u.UserGroups)
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

            if (user.Lockers.Any())
            {
                await NotifyUserLockerAssignment(user, user.Lockers.Select(l => l.Id).ToList(), "add");
            }
        }

        private List<FurchaDAL.Models.Card> AddCardsByNumbers(List<string> cardNumbers, int userId)
        {
            var result = new List<FurchaDAL.Models.Card>();

            try
            {
                if (cardNumbers == null || cardNumbers.Count == 0)
                    return result;

                // normalize + distinct
                var normalized = cardNumbers
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (!normalized.Any())
                    return result;

                var companyId = Db.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.CompanyId)
                    .FirstOrDefault();

                if (companyId == 0)
                    throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "User does not belong to a company.");

                // ✅ conflict only if card exists in same company for ANOTHER user
                var conflict = Db.Cards
                    .Where(c => normalized.Contains(c.CardNumber))
                    .Where(c => c.User.CompanyId == companyId)
                    .Any(c => c.UserId != userId);

                if (conflict)
                {
                    throw new BaseException(
                        ErrorCodeEnum.GenericErrorRetry,
                        "One or more card numbers already exist within this company."
                    );
                }

                // cards that already exist for this user -> skip
                var existingCardNumbers = Db.Cards
                    .Where(c => c.UserId == userId && normalized.Contains(c.CardNumber))
                    .Select(c => c.CardNumber)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (var cardNumber in normalized)
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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, ex.Message);
            }

            return result;
        }

        public async Task SuspendUserGroups(List<int> ids, int state)
        {
            await WithTransactionAsync(async () =>
            {
                var users = Db.UserGroups
                    .Include(ug => ug.Users)
                    .Include(ug => ug.UserGroupLockers)
                    .Where(a => ids.Contains(a.Id)).ToList();

                foreach (var u in users)
                {
                    u.State = state;

                    await NotifyUserGroupLockerAssignment(u, u.UserGroupLockers?.Select(ugl => ugl.LockerId)?.ToList() ?? [], u.State.GetValueOrDefault() == (int)StateEnum.suspended ? "remove" : "add");
                }

                Db.SaveChanges();
            });
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

        public async Task ChangeUsersGroup(List<int> ids, int groupId)
        {
            var group = Db.UserGroups
                .Include(ug => ug.Users)
                .Include(ug => ug.UserGroupLockers)
                .FirstOrDefault(g => g.Id == groupId);
            if (group == null)
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "UserGroup not found");

            await WithTransactionAsync(async () =>
            {
                // Load all target users
                var targetUsers = Db.Users
                    .Include(u => u.UserGroups)
                    .Where(u => ids.Contains(u.Id))
                    .ToList();

                foreach (var user in targetUsers)
                {
                    bool alreadyInGroup = user.UserGroups.Any(g => g.Id == groupId);

                    if (!alreadyInGroup)
                    {
                        user.UserGroups.Add(group); // add only if missing
                    }
                }

                Db.SaveChanges();

                await NotifyUserGroupLockerAssignment(group, group.UserGroupLockers?.Select(ugl => ugl.LockerId)?.ToList() ?? [], "add");
            });
        }


        public SingleUserResult GetUserById(int id)
        {
            var user = Db.Users
                .Include(x => x.UserBranches)
                    .ThenInclude(x => x.Branch)
                        .ThenInclude(x => x.BrainModules)
                            .ThenInclude(b => b.Lockers)
                .Include(u => u.Lockers)
                    .ThenInclude(l => l.Brain)
                        .ThenInclude(l => l.Group)
                .Include(x => x.Cards)
                .Include(u => u.UserGroups)
                .First(u => u.Id == id);

            return new SingleUserResult
            {
                Id = user.Id,
                IsPinRequired = false,
                Name = user.Name,
                Surname = user.Surname,
                Phone = user.Phone,
                Email = user.Email,

                ActiveFrom = Normalize(user.ActiveFrom),
                ActiveTo = Normalize(user.ActiveTo),


                State = (int)user.State,
                UserGroups = user.UserGroups.Select(x => x.Id).ToList(),
                Cards = user.Cards.Select(x => x.CardNumber).ToList(),
                Branches = user.UserBranches.Select(b => new BranchResult
                {
                    Id = b.BranchId,
                    Name = b.Branch.Name,
                    Lockers = user.Lockers
                        .Where(l => l.Brain.BranchId == b.BranchId)
                        .Select(x => x.Id)
                        .ToList(),
                    LockerGroups = user.Lockers
                        .Where(l => l.Brain.BranchId == b.BranchId && l.Brain?.Group != null)
                        .GroupBy(x => x.Brain?.Group?.Name)
                        .Select(g => new LockerGroupResult
                        {
                            LockerGroupName = g.Key,
                            LockersFromGroup = g.Select(lg => new PermittedLockerResult
                            {
                                LockerId = lg.Id,
                                LockerNumber = (long)lg.Number.GetValueOrDefault(0)
                            })
                            .ToList()
                        })
                }).ToList(),
            };
        }


        public GetUserGroupResult? GetUserGroupById(int id)
        {
            var group = Db.UserGroups
             .Include(g => g.UserGroupBranches)
             .FirstOrDefault(g => g.Id == id);

            if (group == null)
                return null;

            var permittedLockers = _lockerService.GetPermittedLockersOfUserGroup(group.Id);

            var branchIds = group.UserGroupBranches
                .Select(ugb => ugb.BranchId)
                .ToList();

            var branches = Db.Branches
                .Where(b => branchIds.Contains(b.Id))
                .ToList();

            var branchResults = group.UserGroupBranches
                .Select(ugb => new UserGroupsBranch
                {
                    Id = ugb.BranchId,
                    Name = branches.First(b => b.Id == ugb.BranchId).Name,
                    Lockers = permittedLockers
                        .Where(l => l.Brain.BranchId == ugb.BranchId)
                        .Select(l => l.Id)
                        .ToList()
                })
                .ToList();

            return new GetUserGroupResult
            {
                Id = id,
                State = (int)group.State,
                Name = group.Name,
                Branches = branchResults
            };
        }

        public async Task<UserGroupResult> EditUserGroup(EditUserGroupRequest req)
        {
            var group = Db.UserGroups
                .Include(g => g.Users)
                .Include(g => g.UserGroupBranches)
                .Include(g => g.UserGroupLockers)
                .FirstOrDefault(g => g.Id == req.Id);

            if (group == null)
                throw new BaseException(ErrorCodeEnum.GenericErrorRetry, "User group not found.");

            UserGroupResult result = null!;

            await WithTransactionAsync(async () =>
            {
                group.Name = req.Name;

                var existingLockerIds = group.UserGroupLockers
                    .Select(x => x.LockerId)
                    .ToList();

                var lockersToRemove = group.UserGroupLockers
                    .Where(ugl => !req.LockerIds.Contains(ugl.LockerId))
                    .ToList();

                if (lockersToRemove.Any())
                    Db.UserGroupLockers.RemoveRange(lockersToRemove);

                var lockersToAdd = req.LockerIds
                    .Where(lockerId => !existingLockerIds.Contains(lockerId))
                    .Select(lockerId => new UserGroupLocker
                    {
                        UserGroupId = group.Id,
                        LockerId = lockerId
                    })
                    .ToList();

                if (lockersToAdd.Any())
                    Db.UserGroupLockers.AddRange(lockersToAdd);

                Db.SaveChanges();

                var branchIds = Db.Lockers
                    .Where(l => req.LockerIds.Contains(l.Id))
                    .Select(l => l.Brain.BranchId)
                    .Where(b => b.HasValue)
                    .Select(b => b.Value)
                    .Distinct()
                    .ToList();

                var branchesToRemove = group.UserGroupBranches
                    .Where(ugb => !branchIds.Contains(ugb.BranchId))
                    .ToList();

                if (branchesToRemove.Any())
                    Db.UserGroupBranches.RemoveRange(branchesToRemove);

                // Add new branches
                var branchesToAdd = branchIds
                    .Where(branchId => !group.UserGroupBranches.Any(ugb => ugb.BranchId == branchId))
                    .Select(branchId => new UserGroupBranch
                    {
                        UserGroupId = group.Id,
                        BranchId = branchId
                    })
                    .ToList();

                if (branchesToAdd.Any())
                    Db.UserGroupBranches.AddRange(branchesToAdd);

                Db.SaveChanges();

                var branchNames = Db.Branches
                    .Where(b => branchIds.Contains(b.Id))
                    .Select(b => b.Name)
                    .ToList();

                var lockerGroups = Db.LockerGroups
                    .Include(lg => lg.BrainModules)
                        .ThenInclude(bm => bm.Lockers)
                    .Where(lg =>
                        lg.BrainModules
                            .SelectMany(b => b.Lockers)
                            .Any(l => req.LockerIds.Contains(l.Id)))
                    .ToList();

                var lockerGroupResults = lockerGroups
                    .Select(lg => new LockerGroupResult
                    {
                        LockerGroupName = lg.Name,
                        LockersFromGroup = lg.BrainModules
                            .SelectMany(b => b.Lockers)
                            .Where(l => req.LockerIds.Contains(l.Id))
                            .Select(l => new PermittedLockerResult
                            {
                                LockerId = l.Id,
                                LockerNumber = (int)l.Number
                            })
                            .ToList()
                    })
                    .ToList();

                await NotifyUserGroupLockerAssignment(group, group.UserGroupLockers?.Select(ugl => ugl.LockerId)?.ToList() ?? [], group.State.GetValueOrDefault() == (int)StateEnum.suspended ? "remove" : "add");

                result = new UserGroupResult
                {
                    Id = group.Id,
                    Name = group.Name,
                    State = (int)group.State,
                    BranchNames = branchNames,
                    PermittedLockers = lockerGroupResults,
                    UserCount = Db.Users.Count(u => u.UserGroups.Any(ug => ug.Id == group.Id))
                };
            });

            return result;
        }



    }
}
