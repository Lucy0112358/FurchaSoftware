using Domain.Enums;
using Domain.Extensions;

namespace Domain.Exceptionss
{
    public class BaseException : Exception
    {
        public MqttErrorCodeEnum ErrorCodeEnum { get; private set; }
        public ErrorCodeEnum errorCodeEnum { get; private set; }
        public BaseException()
        {
        }
        public BaseException(MqttErrorCodeEnum errorCodeType) : base(errorCodeType.GetDescription())
        {
            ErrorCodeEnum = errorCodeType;
        }
        public BaseException(ErrorCodeEnum errorCodeType, string message, int? id = null) : base(message)
        {
            ErrorCodeEnum = (MqttErrorCodeEnum)errorCodeType;
        }
        public BaseException(ErrorCodeEnum errorCodeType) : base(errorCodeType.GetDescription())
        {
            errorCodeEnum = errorCodeType;
        }
    }
}
