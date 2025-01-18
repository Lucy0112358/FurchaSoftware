using Domain.Entities;
using FurchaAdminApi.Models.Request;
using FurchaAdminApi.Models.Result;
using FurchaAdminApi.Repos;
using System.Transactions;

namespace FurchaAdminApi.Services
{
    public class BranchService
    {
        private readonly BranchRepository _branchRepository;
        private readonly UserRepository _userRepository;

        public BranchService(BranchRepository branchRepository, UserRepository userRepository)
        {
            _branchRepository = branchRepository;
            _userRepository = userRepository;
        }

        public List<Branch> GetAllBranches(int adminId)
        {
            var companyId = _userRepository.GetCompanyIdByAdminId(adminId);

            return _branchRepository.GetAllBranches(companyId);
        }

        public List<Branch> GetSearchedBranches(string name, int adminId)
        {
            return _branchRepository.SearchBranchByAdminId(name, adminId);
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
                    AddressId = address.Id
                });
                return true;

            

        }
    }
}
