namespace SupplierGuard.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity, IAuditableEntity
    {
        /// <summary>
        /// Date and time of creation of the entity (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who created the entity.
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Date and time of last modification (UTC).
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// User who made the last modification.
        /// </summary>
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Protected constructor that initializes the creation date.
        /// </summary>
        protected AuditableEntity() : base()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Builder with specific ID.
        /// </summary>
        protected AuditableEntity(Guid id) : base(id)
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
