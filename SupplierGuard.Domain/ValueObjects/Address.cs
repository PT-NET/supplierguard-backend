namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a physical address.
    /// </summary>
    public sealed class Address : IEquatable<Address>
    {
        /// <summary>
        /// Complete physical address.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private Address(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated Address.
        /// </summary>
        /// <param name="value">Physical address.</param>
        /// <returns>Address instance.</returns>
        /// <exception cref="ArgumentException">If the address is not valid.</exception>
        public static Address Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Address cannot be empty.", nameof(value));

            value = value.Trim();

            if (value.Length < 5)
                throw new ArgumentException("Address must be at least 5 characters long.", nameof(value));

            if (value.Length > 500)
                throw new ArgumentException("Address cannot exceed 500 characters.", nameof(value));

            return new Address(value);
        }

        /// <summary>
        /// Checks if the address contains a specific text.
        /// </summary>
        public bool Contains(string text)
        {
            return Value.Contains(text, StringComparison.OrdinalIgnoreCase);
        }

        #region Equality

        public bool Equals(Address? other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return obj is Address address && Equals(address);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(Address? left, Address? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Address? left, Address? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from Address to string.
        /// </summary>
        public static implicit operator string(Address address) => address.Value;
    }
}
