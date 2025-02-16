using Domain.Entities;
using Domain.Enums;
using FurchaAdminApi.Mappers;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using System.Net;

namespace FurchaAdminApi.Services
{
    public class BranchService
    {
        private readonly BranchRepository _branchRepository;
        private readonly UserRepository _userRepository;
        private readonly LockerRepository _lockerRepository;

        public BranchService(BranchRepository branchRepository, UserRepository userRepository, LockerRepository lockerRepository)
        {
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _lockerRepository = lockerRepository;
        }

        public List<AllBranchResult> GetAllBranches(int adminId)
        {
            var companyId = _userRepository.GetCompanyIdByAdminId(adminId);
          
            return _branchRepository.GetAllBranches(companyId)
                .Select(x => x.ToAllBranchResult(_branchRepository.GetBranchAddressById(x.AddressId).Street, _branchRepository.GetLockerTypesByBranch(x.Id), GetLocersCount(x.Id))).ToList();
        }

        private int GetLocersCount(int branchId)
        {
           var count = _lockerRepository.GetLockersByBranchId(branchId).Count;

            return count;
        }

        public List<AllBranchResult> GetSearchedBranches(string name, int adminId)
        {
            return _branchRepository.SearchBranchByAdminId(name, adminId)
                .Select(x => x.ToAllBranchResult(_branchRepository.GetBranchAddressById(x.AddressId).Street, _branchRepository.GetLockerTypesByBranch(x.Id), GetLocersCount(x.Id))).ToList();
        }

        public bool CreateBranch(CreateBranchRequest newBranch, int adminId)
        {          
                var address = _branchRepository.CreateBranchAddress(new BranchAddress
                {
                    Street = newBranch.Address,
                    City = newBranch.Address,
                    PostalCode = "1",
                    Country = "1"
                });

                var branch = _branchRepository.CreateBranch(new Branch
                {
                    Name = newBranch.BranchName,
                    CompanyId = _userRepository.GetCompanyIdByAdminId(adminId),
                    AddressId = address.Id,
                    Comment = newBranch.Comment,
                    Mode = (int)StateEnum.active
                });

                return true;           
        }
    }
}
