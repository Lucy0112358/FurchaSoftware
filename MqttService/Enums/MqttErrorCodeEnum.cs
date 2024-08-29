using System.ComponentModel;

namespace MqttService.Enums
{
    public enum MqttErrorCodeEnum : long
    {
        [Description("An error occured, please try again later")]
        GenericErrorRetry = 1,

        [Description("Account has been deleted and cannot be used anymore")]
        UserAccountIsDeleted = 2,

        [Description("The password is invalid. Please make sure you request the right locker or contact support.")]
        InvalidPassword = 11,

        [Description("The card is invalid. Please make sure you request the right locker or contact support.")]
        InvalidCard = 12,

        [Description("Success.")]
        Success = 100,

    }
}
