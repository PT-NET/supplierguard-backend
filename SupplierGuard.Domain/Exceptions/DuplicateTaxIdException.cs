namespace SupplierGuard.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when attempting to create a supplier with a Tax ID that already exists.
    /// </summary>
    public sealed class DuplicateTaxIdException : DomainException
    {
        public string TaxId { get; }

        public DuplicateTaxIdException(string taxId)
            : base($"A supplier with Tax ID '{taxId}' already exists.")
        {
            TaxId = taxId;
        }
    }
}
