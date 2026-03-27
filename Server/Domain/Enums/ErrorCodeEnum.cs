using System.ComponentModel;

namespace Domain.Enums
{
    public enum ErrorCodeEnum : long
    {     

        /// <summary>
        /// We try to avoid telling the user whether the username or password is wrong, to discourage trial and error.
        /// </summary>
        [Description("The username or password is wrong.")]
        WrongUsernameOrPassword = 28,

        [Description("The user is trying to login using a Microsoft provider email. Redirect to provider login.")]
        LoginWithMicrosoftEmail = 10301,
        [Description("The input must not contain HTML.")]
        InputPotentiallyDangerous = 3701,
        /// <summary>
        /// If Admin's role is not super or master, they can only be assigned to 1 branch.
        /// We show this error, if they have access to more than 1 branch
        /// </summary>
        [Description("Something is wrong. This level of admin can only have 1 branch assigned.")]
        AdminHasMoreBranchesThanPermitted = 10302,

        [Description("An error occured, please try again later")]
        GenericErrorRetry = 1,

        [Description("A user with this email already exists in your company.")]
        EmailAlreadyExists = 2,

        [Description("A locker group with this name already exists.")]
        LockerGroupNameExists = 3,

        [Description("A group with this name already exists.")]
        UserGroupNameExists = 4,

    }
}
