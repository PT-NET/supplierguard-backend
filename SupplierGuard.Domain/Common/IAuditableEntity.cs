namespace SupplierGuard.Domain.Common
{
    /// <summary>
    /// Interface for entities that require creation and modification auditing.
    /// </summary>
    public interface IAuditableEntity
    {
        /// <summary>
        /// Date and time of creation of the entity (UTC).
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who created the entity (optional).
        /// </summary>
        string? CreatedBy { get; set; }

        /// <summary>
        /// Date and time of last modification (UTC).
        /// </summary>
        DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// User who made the last modification (optional).
        /// </summary>
        string? LastModifiedBy { get; set; }
    }
}
