namespace SupplierGuard.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when there are validation errors.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Dictionary of errors by field.
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        public ValidationException(string propertyName, string errorMessage)
            : this()
        {
            Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        };
        }
    }

    /// <summary>
    /// Exception thrown when a resource is not found.
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException()
            : base()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }

    /// <summary>
    /// Exception thrown when there is a conflict (e.g., duplicate RUT).
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException()
            : base()
        {
        }

        public ConflictException(string message)
            : base(message)
        {
        }

        public ConflictException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a business operation is invalid.
    /// </summary>
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
