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
            var companyId = Db.Administrators
                .Where(a => a.Id == adminId)
                .First()
                .CompanyId;

            return Db.Branches
                .Where(b => b.CompanyId == companyId)
                .Select(x => x.ToAllBranchResult(
                    Db.BranchAddresses
                        .Where(a => a.Id == x.AddressId)
                        .FirstOrDefault().Street,
                    Db.Lockers
                        .Where(l => l.BranchId == x.Id)
                        .Select(l => l.LockerType)
                        .Distinct()
                        .ToList(),
                    Db.Lockers
                        .Where(c => c.BranchId == x.Id)
                        .Count()))
                .ToList(); 
        }

        private int GetLocersCount(int branchId)
        {
            var count = Db.Lockers.Where(l => l.BranchId == branchId).Count(); //_lockerRepository.GetLockersByBranchId(branchId).Count;

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

            return branches// _branchRepository.SearchBranchByAdminId(name, adminId)
              .Select(x => x.ToAllBranchResult(
                    Db.BranchAddresses
                        .Where(a => a.Id == x.AddressId)
                        .FirstOrDefault().Street,
                    Db.Lockers
                        .Where(l => l.BranchId == x.Id)
                        .Select(l => l.LockerType)
                        .Distinct()
                        .ToList(),
                    Db.Lockers
                        .Where(c => c.BranchId == x.Id)
                        .Count()))
                .ToList();   //.Select(x => x.ToAllBranchResult(_branchRepository.GetBranchAddressById(x.AddressId).Street, _branchRepository.GetLockerTypesByBranch(x.Id), GetLocersCount(x.Id))).ToList();
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
                CompanyId = _userRepository.GetCompanyIdByAdminId(adminId),
                AddressId = address.Entity.Id,
                Comment = newBranch.Comment,
                Mode = (int)StateEnum.active
            });

            var res = Db.SaveChanges();
            return res > 0;
        }

        public bool EditBranch(CreateBranchRequest newBranch, int id)
        {
            var branch = Db.Branches.FirstOrDefault(b => b.Id == id);

            if (branch == null)
                return false;

            branch.Name = newBranch.Name;
            branch.Comment = newBranch.Comment;

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
