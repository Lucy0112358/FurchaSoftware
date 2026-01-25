using FurchaAdminApi.Models.Result;
using FurchaDAL.Models;

namespace FurchaAdminApi.Mappers
{
    public static class BranchMappers
    {
        /*        public static AllBranchResult ToAllBranchResult(this Branch branch, string address, List<string> lockerTypes, int count)
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
                }*/
        public static AllBranchResult ToAllBranchResult(
      this FurchaDAL.Models.Branch branch,
      string street,
      List<LockerTypeResult> lockerTypes,
      int lockersCount)
        {
            return new AllBranchResult
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = street,
                Comment = branch.Comment,
                LockerTypes = lockerTypes,
                LockersCount = lockersCount,
                Mode = branch.Mode ?? 0   // 👈 FIXED
            };
        }


    }
}
