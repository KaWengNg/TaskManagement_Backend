using TaskManagment.Enum;

namespace TaskManagment.Services
{
    public class ServiceResult<T>
    {
        public ServiceStatus Status { get; }
        public T? Data { get; }
        public string? Message { get; }

        private ServiceResult(ServiceStatus status, T? data = default, string? message = null)
        {
            Status = status;
            Data = data;
            Message = message;
        }

        public static ServiceResult<T> Success(T data, string? message = null) =>
            new(ServiceStatus.Success, data, message);

        public static ServiceResult<T> NotFound(string? message = null) =>
            new(ServiceStatus.NotFound, default, message);

        public static ServiceResult<T> ValidationError(string? message = null) =>
            new(ServiceStatus.ValidationError, default, message);

        public static ServiceResult<T> Error(string? message = null) =>
            new(ServiceStatus.ServerError, default, message);
    }
}

