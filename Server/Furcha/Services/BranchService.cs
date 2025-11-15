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
        private readonly LockerRepository _lockerRepository;
        private readonly furchaContext Db;

        public BranchService(BranchRepository branchRepository, UserRepository userRepository, LockerRepository lockerRepository, furchaContext db)
        {
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _lockerRepository = lockerRepository;
            Db = db;
        }

        public List<AllBranchResult> GetAllBranches(int adminId)
        {
            return Db.AdminBranches
                .Where(ab => ab.AdministratorId == adminId)
                .Select(ab => ab.Branch) // ← gets the related Branch entity
                .Select(branch => branch.ToAllBranchResult(

                    // Address
                    Db.BranchAddresses
                        .Where(a => a.Id == branch.AddressId)
                        .Select(a => a.Street)
                        .FirstOrDefault(),

                    // Locker Types
                    Db.Lockers
                        .Include(l => l.Brain)
                        .Include(l => l.LockerTypeNavigation)
                        .Where(l => l.Brain.BranchId == branch.Id)
                        .Select(l => new LockerTypeResult
                        {
                            Id = l.LockerTypeNavigation.Id,
                            Name = l.LockerTypeNavigation.Name
                        })
                        .Distinct()
                        .ToList(),

                    // Lockers count
                    Db.Lockers
                        .Include(l => l.Brain)
                        .Count(l => l.Brain.BranchId == branch.Id)
                ))
                .ToList();
        }

        private int GetLocersCount(int branchId)
        {
            var count = Db.Lockers.Include(x => x.Brain).Where(l => l.Brain.BranchId == branchId).Count(); //_lockerRepository.GetLockersByBranchId(branchId).Count;

            return count;
        }

        public List<AllBranchResult> GetSearchedBranches(string name, int adminId)
        {
            var companyId = Db.Administrators
                               .Where(a => a.Id == adminId)
                               .Select(a => a.CompanyId)
                               .FirstOrDefault();

            var branches = Db.Branches
                .Where(b => b.CompanyId == companyId &&
                            b.Name.ToLower().Contains(name.ToLower()))
                .ToList();

            return branches
                .Select(x => x.ToAllBranchResult(
                    Db.BranchAddresses
                        .Where(a => a.Id == x.AddressId)
                        .Select(a => a.Street)
                        .FirstOrDefault(),

                    Db.Lockers
                        .Include(l => l.Brain)
                        .Include(l => l.LockerTypeNavigation)
                        .Where(l => l.Brain.BranchId == x.Id)
                        .Select(l => new LockerTypeResult
                        {
                            Id = l.LockerTypeNavigation.Id,
                            Name = l.LockerTypeNavigation.Name
                        })
                        .Distinct()
                        .ToList(),

                    Db.Lockers
                        .Include(l => l.Brain)
                        .Count(l => l.Brain.BranchId == x.Id)
                ))
                .ToList();
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
                CompanyId = Db.Administrators.FirstOrDefault(a => a.Id == adminId).CompanyId, //_userRepository.GetCompanyIdByAdminId(AdminId),
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

            if (branch == null)
                return false;

            branch.Name = newBranch.Name;
            branch.Comment = newBranch.Comment;
            if (branch.BranchAddresses.FirstOrDefault() != null && !string.IsNullOrEmpty(newBranch.Address))
            {
                branch.BranchAddresses.FirstOrDefault().Street = newBranch.Address;
            }

            var res = Db.SaveChanges();

            return res > 0;
        }

        public void DeleteBranch(int branchId)
        {
            var branch = Db.Branches
                .Include(b => b.AdminBranches)
                .Include(b => b.BranchAddresses)
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

            Db.Branches.Remove(branch);
            Db.SaveChanges();
        }


    }
}
