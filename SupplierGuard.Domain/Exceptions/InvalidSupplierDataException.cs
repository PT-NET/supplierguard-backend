namespace SupplierGuard.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a provider's data is invalid.
    /// </summary>
    public sealed class InvalidSupplierDataException : DomainException
    {
        public string PropertyName { get; }

        public InvalidSupplierDataException(string propertyName, string message)
            : base($"Invalid value for {propertyName}: {message}")
        {
            PropertyName = propertyName;
        }

        public InvalidSupplierDataException(string message)
            : base(message)
        {
            PropertyName = string.Empty;
        }
    }
}
