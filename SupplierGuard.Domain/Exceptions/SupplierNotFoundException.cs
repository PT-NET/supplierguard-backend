namespace SupplierGuard.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a provider is not found.
    /// </summary>
    public sealed class SupplierNotFoundException : DomainException
    {
        public Guid SupplierId { get; }

        public SupplierNotFoundException(Guid supplierId)
            : base($"Supplier with ID '{supplierId}' was not found.")
        {
            SupplierId = supplierId;
        }

        public SupplierNotFoundException(string taxId)
            : base($"Supplier with Tax ID '{taxId}' was not found.")
        {
        }
    }
}
