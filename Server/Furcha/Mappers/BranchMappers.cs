using Domain.Entities;
using FurchaAdminApi.Models.Result;

namespace FurchaAdminApi.Mappers
{
    public static class BranchMappers
    {
        public static AllBranchResult ToAllBranchResult(this Branch branch, string address, List<string> lockerTypes, int count)
        {
            return new AllBranchResult
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = address,
                LockerTypes = lockerTypes,
                Comment = branch.Comment,
                LockersCount = count,
                Mode = branch.Mode,
            };
        }
    }
}
