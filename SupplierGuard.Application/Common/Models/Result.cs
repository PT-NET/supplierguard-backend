namespace SupplierGuard.Application.Common.Models
{
    /// <summary>
    /// Represents the result of an operation with the Result pattern.
    /// It allows you to handle success/failure without exceptions.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// List of errors if the operation failed.
        /// </summary>
        public List<string> Errors { get; private set; }

        /// <summary>
        /// First mistake (for convenience).
        /// </summary>
        public string Error => Errors.FirstOrDefault() ?? string.Empty;

        protected Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToList();
        }

        /// <summary>
        /// Create a successful outcome.
        /// </summary>
        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        /// <summary>
        /// Create a failed result with an error.
        /// </summary>
        public static Result Failure(string error)
        {
            return new Result(false, new[] { error });
        }

        /// <summary>
        /// Create a failed result with multiple errors.
        /// </summary>
        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }
    }

    /// <summary>
    /// It represents the result of an operation that returns a value.
    /// </summary>
    public class Result<T> : Result
    {
        /// <summary>
        /// Value returned by the operation (if it was successful).
        /// </summary>
        public T? Data { get; private set; }

        protected Result(bool succeeded, T? data, IEnumerable<string> errors)
            : base(succeeded, errors)
        {
            Data = data;
        }

        /// <summary>
        /// Create a successful outcome with data.
        /// </summary>
        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, Array.Empty<string>());
        }

        /// <summary>
        /// Create a failed result with an error.
        /// </summary>
        public new static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, new[] { error });
        }

        /// <summary>
        /// Create a failed result with multiple errors.
        /// </summary>
        public new static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(false, default, errors);
        }
    }
}
