using Domain.Enums;
using MqttService.Extensions;
using System.Diagnostics.Contracts;

namespace MqttService.Application.Exceptions
{
    public class BaseException : ApplicationException
    {
        public MqttErrorCodeEnum ErrorCodeEnum { get; private set; }
        public BaseException()
        {
        }
        public BaseException(MqttErrorCodeEnum errorCodeType) : base(errorCodeType.GetDescription())
        {
            ErrorCodeEnum = errorCodeType;
        }
    }
}
