namespace SupplierGuard.Domain.Common
{
    /// <summary>
    /// Base class for all entities in the domain.
    /// It provides a unique identity (Id) to each entity.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier of the entity.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Protected constructor to ensure that entities are created with a valid ID.
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructor that allows you to specify an Id (useful for testing or rebuilding from DB).
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        protected BaseEntity(Guid id)
        {
            Id = id;
        }

        /// <summary>
        /// Determine if two entities are equal based on their ID.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Gets the hash code based on the Id.
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Equality Operator.
        /// </summary>
        public static bool operator ==(BaseEntity? left, BaseEntity? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(BaseEntity? left, BaseEntity? right)
        {
            return !(left == right);
        }
    }
}
