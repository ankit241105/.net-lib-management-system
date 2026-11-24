namespace lib_management_system.Common
{
    // A small result wrapper so a service can report success OR the correct HTTP status + message,
    // which keeps controllers thin (they just translate this into an IActionResult).
    public class ServiceResult<T>
    {
        public bool Success { get; init; }
        public int StatusCode { get; init; }
        public string? Error { get; init; }
        public T? Data { get; init; }

        public static ServiceResult<T> Ok(T data, int statusCode = 200)
            => new() { Success = true, StatusCode = statusCode, Data = data };

        public static ServiceResult<T> Fail(int statusCode, string error)
            => new() { Success = false, StatusCode = statusCode, Error = error };
    }
}
