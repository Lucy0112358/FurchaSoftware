namespace FurchaBLL.Constants
{
    public enum CommandTypes
    {
        UpdateDoorStatus = 1,
        OpenLockersFromAdmin,
        SynchronizeUsers,
        CreateUserFromAdmin,
        CreateAccount,
        CreateBrainModule,
        AddLockersToBrain,
        SendAuthDataForOpen,
        SuspendUser,
        SuspendLockerDoor,
        OpenDoorsFromAdmin,
        SendCard,
        ChangeType,
        StatusActiveSuspend,
        SendLockerMode,
        AssigningUserToLocker,
        SendStatus,            
        AssigningUserGroupToLocker   /* = 18 */
    }
}
