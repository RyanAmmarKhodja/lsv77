using MailKit;

namespace campus_insider.Services
{
    public class ServiceResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? Metadata { get; init; }
        public string? ErrorMessage { get; init; }

        public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
        public static ServiceResult<T> Fail(string error) => new() { Success = false, ErrorMessage = error };
    }

    public class ServiceResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }

        public static ServiceResult Ok() => new() { Success = true };
        public static ServiceResult Fail(string error) => new() { Success = false, ErrorMessage = error };
    }
}
