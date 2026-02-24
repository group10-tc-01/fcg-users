using System.Net;

namespace FCG.Users.Application.Abstractions.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }

        private Result(bool isSuccess, T? value, string? errorMessage, HttpStatusCode statusCode)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public static Result<T> Success(T value) => new(true, value, null, HttpStatusCode.OK);

        public static Result<T> Failure(string errorMessage, HttpStatusCode statusCode) => new(false, default, errorMessage, statusCode);
    }
}
