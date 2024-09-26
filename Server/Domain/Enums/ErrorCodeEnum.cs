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

        [Description("An error occured, please try again later")]
        GenericErrorRetry = 1

    }
}
