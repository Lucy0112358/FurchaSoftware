using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PermissionEnum : long
    {
        CreateAdmin = 1,
        InteractWithUsers,
        InteractWithUserGroups,
        ImportExportUsers,
        AddModules,
        CreateLockerGroup,
        StoreParcel
            // assign temp perc to common lockers, ask
    }
}
