namespace B2B_Proje.Business.DTOs
{
    public class ApiResponseDto<T>
    {
        public T? Data { get; init; }
        public bool HasError { get; init; }
        public string? ErrorType { get; init; }
        public string? Message { get; init; }
        public IReadOnlyDictionary<string, string[]>? Errors { get; init; }

        public static ApiResponseDto<T> Success(T? data, string? message = null)
        {
            return new ApiResponseDto<T>
            {
                Data = data,
                HasError = false,
                ErrorType = null,
                Message = message
            };
        }

        public static ApiResponseDto<T> Failure(
            string errorType,
            string message,
            IReadOnlyDictionary<string, string[]>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Data = default,
                HasError = true,
                ErrorType = errorType,
                Message = message,
                Errors = errors
            };
        }
    }
}
