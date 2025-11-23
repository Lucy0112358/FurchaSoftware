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
        SuspendLocker,       
        AddUser
    }
}
