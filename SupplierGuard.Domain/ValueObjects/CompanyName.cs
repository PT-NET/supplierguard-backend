namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a company name (Legal Name or Trade Name).
    /// </summary>
    public sealed class CompanyName : IEquatable<CompanyName>
    {
        /// <summary>
        /// Company name.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private CompanyName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated CompanyName.
        /// </summary>
        /// <param name="value">Company name.</param>
        /// <returns>CompanyName instance.</returns>
        /// <exception cref="ArgumentException">If the name is not valid.</exception>
        public static CompanyName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Company name cannot be empty.", nameof(value));

            value = value.Trim();

            if (value.Length < 2)
                throw new ArgumentException("Company name must be at least 2 characters long.", nameof(value));

            if (value.Length > 200)
                throw new ArgumentException("Company name cannot exceed 200 characters.", nameof(value));

            return new CompanyName(value);
        }

        /// <summary>
        /// Converts the name to uppercase.
        /// </summary>
        public string ToUpperCase()
        {
            return Value.ToUpperInvariant();
        }

        /// <summary>
        /// Checks if the name contains a specific text.
        /// </summary>
        public bool Contains(string text)
        {
            return Value.Contains(text, StringComparison.OrdinalIgnoreCase);
        }

        #region Equality

        public bool Equals(CompanyName? other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return obj is CompanyName companyName && Equals(companyName);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(CompanyName? left, CompanyName? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(CompanyName? left, CompanyName? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from CompanyName to string.
        /// </summary>
        public static implicit operator string(CompanyName companyName) => companyName.Value;
    }
}
