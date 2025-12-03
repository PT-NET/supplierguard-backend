namespace SupplierGuard.Models
{
    /// <summary>
    /// Standardized API response.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Descriptive message of the result.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Response data (if successful).
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error list (if the operation failed).
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Response timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Create a successful response.
        /// </summary>
        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Create an error response.
        /// </summary>
        public static ApiResponse<T> Fail(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        /// <summary>
        /// Create an error response with multiple errors.
        /// </summary>
        public static ApiResponse<T> Fail(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = errors
            };
        }

        /// <summary>
        /// Create an error response with a validation dictionary.
        /// </summary>
        public static ApiResponse<T> Fail(IDictionary<string, string[]> validationErrors)
        {
            var errors = validationErrors
                .SelectMany(kvp => kvp.Value.Select(error => $"{kvp.Key}: {error}"))
                .ToList();

            return new ApiResponse<T>
            {
                Success = false,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Response without data (for operations that do not return content).
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Ok(string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        public new static ApiResponse Fail(string error)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        public new static ApiResponse Fail(List<string> errors)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = errors
            };
        }
    }
}
