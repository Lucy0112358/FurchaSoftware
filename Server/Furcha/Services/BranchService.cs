using Domain.Enums;
using FurchaAdminApi.Mappers;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using FurchaDAL.Models;
using Microsoft.EntityFrameworkCore;
using Branch = FurchaDAL.Models.Branch;

namespace FurchaAdminApi.Services
{
    public class BranchService
    {
        private readonly BranchRepository _branchRepository;
        private readonly UserRepository _userRepository;
        private readonly LockerService _lockerService;
        private readonly LockerRepository _lockerRepository;
        private readonly furchaContext Db;

        public BranchService(BranchRepository branchRepository, UserRepository userRepository, LockerRepository lockerRepository, furchaContext db, LockerService lockerService)
        {
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _lockerRepository = lockerRepository;
            Db = db;
            _lockerRepository = lockerRepository;
        }

        public List<AllBranchResult> GetBranches(int adminId, string? name)
        {
            var adminBranchIds = Db.AdminBranches
                .Where(ab => ab.AdministratorId == adminId)
                .Select(ab => ab.BranchId)
                .ToList();

            var query = Db.Branches
                .Where(b => adminBranchIds.Contains(b.Id));

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(b => EF.Functions.Like(b.Name, $"%{name}%"));
            }

            var branches = query.ToList();

            return branches
                .Select(b => BuildBranchResult(b))
                .ToList();
        }

        private AllBranchResult BuildBranchResult(Branch branch)
        {
            var street = Db.BranchAddresses
                .Where(a => a.Id == branch.AddressId)
                .Select(a => a.Street)
                .FirstOrDefault();

            var lockerTypes = Db.Lockers
                .Include(l => l.Brain)
                .Include(l => l.LockerTypeNavigation)
                .Where(l => l.Brain.BranchId == branch.Id)
                .Select(l => new LockerTypeResult
                {
                    Id = l.LockerTypeNavigation.Id,
                    Name = l.LockerTypeNavigation.Name
                })
                .Distinct()
                .ToList();

            var lockerCount = Db.Lockers
                .Include(l => l.Brain).Where(l => l.IsDeleted == false)
                .Count(l => l.Brain.BranchId == branch.Id);

            return branch.ToAllBranchResult(
                street,
                lockerTypes,
                lockerCount
            );
        }


        private int GetLocersCount(int branchId)
        {
            var count = Db.Lockers.Include(x => x.Brain).Where(l => l.Brain.BranchId == branchId).Count();

            return count;
        }

        public bool CreateBranch(CreateBranchRequest newBranch, int adminId)
        {
            var address = Db.BranchAddresses.Add(new FurchaDAL.Models.BranchAddress
            {
                Street = newBranch.Address,
                City = newBranch.Address,
                PostalCode = "1",
                Country = "1"
            });

            Db.SaveChanges();

            var branch = Db.Branches.Add(new FurchaDAL.Models.Branch
            {
                Name = newBranch.Name,
                CompanyId = Db.Administrators.FirstOrDefault(a => a.Id == adminId).CompanyId,
                AddressId = address.Entity.Id,
                Comment = newBranch.Comment,
                Mode = (int)StateEnum.active
            });

            var res = Db.SaveChanges();

            var adminBranch = new AdminBranch
            {
                AdministratorId = adminId,
                BranchId = branch.Entity.Id
            };

            Db.AdminBranches.Add(adminBranch);
            Db.SaveChanges();

            return res > 0;
        }

        public bool EditBranch(CreateBranchRequest newBranch, int id)
        {
            var branch = Db.Branches.Include(b => b.BranchAddresses).FirstOrDefault(b => b.Id == id);
            var branchAddress = Db.BranchAddresses.Where(a => a.Id == branch.AddressId).FirstOrDefault();

            if (branch == null)
                return false;

            branch.Name = newBranch.Name;
            branch.Comment = newBranch.Comment;
            if (branchAddress != null && !string.IsNullOrEmpty(newBranch.Address))
            {
                branchAddress.Street = newBranch.Address;
            }

            var res = Db.SaveChanges();

            return res > 0;
        }

        public void DeleteBranch(int branchId)
        {
            var branch = Db.Branches
                .Include(b => b.AdminBranches)
                .Include(b => b.BranchAddresses)
                .Include(b => b.BrainModules)
                .FirstOrDefault(b => b.Id == branchId);

            if (branch == null)
                return;

            if (branch.AdminBranches != null)
            {
                Db.AdminBranches.RemoveRange(branch.AdminBranches);
            }

            if (branch.BranchAddresses != null)
            {
                Db.BranchAddresses.RemoveRange(branch.BranchAddresses);
            }

            foreach (var m in branch.BrainModules)
            {
                _lockerService.DeleteModule(m.Id);
            }

            Db.Branches.Remove(branch);
            Db.SaveChanges();
        }


    }
}
