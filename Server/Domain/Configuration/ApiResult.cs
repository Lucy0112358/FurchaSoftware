using Domain.Enums;
using Domain.Extensions;

namespace Domain.Configuration
{
    /// <summary>
    /// An api result which indicates api call success and can contain error information.
    /// </summary>
    public class ApiResult
    {
        public bool isSuccess { get; set; }
        public ErrorCodeEnum? error { get; set; }

        protected string? errorMessage;
        public string ErrorMessage
        {
            get
            {
                if (errorMessage.IsNotNullOrEmpty())
                {
                    return errorMessage;
                }

                if (error.HasValue)
                {
                    return (long)error == 0 ? string.Empty : error.GetDescription();
                }

                return string.Empty;
            }
        }

        public static ApiResult Success()
        {
            return new ApiResult { isSuccess = true, error = null };
        }

        public static ApiResult ErrorResult(ErrorCodeEnum error = ErrorCodeEnum.GenericErrorRetry)
        {
            return new ApiResult { isSuccess = false, error = error };
        }

        public static ApiResult ErrorResult(string errorMessage)
        {
            return new ApiResult { isSuccess = false, errorMessage = errorMessage };
        }

        public static ApiResult ErrorResult(ErrorCodeEnum error, string errorMessage)
        {
            return new ApiResult { isSuccess = false, error = error, errorMessage = errorMessage };
        }
    }

    /// <summary>
    /// A generic implementation of <see cref="ApiResult"/> which can contain returned value by the server.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResult<T> : ApiResult
    {
        public T? data { get; set; }

        public static ApiResult<T> Success(T result)
        {
            return new ApiResult<T> { isSuccess = true, error = null, data = result };
        }

        public new static ApiResult<T> ErrorResult(ErrorCodeEnum error = ErrorCodeEnum.GenericErrorRetry)
        {
            return new ApiResult<T> { isSuccess = false, error = error, data = default };
        }

        public new static ApiResult<T> ErrorResult(string errorMessage)
        {
            return new ApiResult<T> { isSuccess = false, errorMessage = errorMessage, data = default };
        }

        public static ApiResult<T> ErrorResult(string errorMessage, T data)
        {
            return new ApiResult<T> { isSuccess = false, errorMessage = errorMessage, data = data };
        }

        public new static ApiResult<T> ErrorResult(ErrorCodeEnum error, string errorMessage)
        {
            return new ApiResult<T> { isSuccess = false, error = error, errorMessage = errorMessage, data = default };
        }
    }

}
