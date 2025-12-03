namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a RUT/NIT (Tax Identification Number).
    /// Must have exactly 11 numeric digits.
    /// </summary>
    public sealed class TaxId : IEquatable<TaxId>
    {
        /// <summary>
        /// RUT/NIT value (11 digits).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private TaxId(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated TaxId.
        /// </summary>
        /// <param name="value">11-digit RUT/NIT.</param>
        /// <returns>TaxId instance.</returns>
        /// <exception cref="ArgumentException">If the value is not valid.</exception>
        public static TaxId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Tax ID cannot be empty.", nameof(value));

            // Remove dashes, spaces and other characters
            var cleanValue = new string(value.Where(char.IsDigit).ToArray());

            if (cleanValue.Length != 11)
                throw new ArgumentException("Tax ID must be exactly 11 digits.", nameof(value));

            return new TaxId(cleanValue);
        }

        /// <summary>
        /// Returns the formatted TaxId (XX.XXX.XXX-X).
        /// </summary>
        public string Formatted()
        {
            if (Value.Length != 11)
                return Value;

            return $"{Value.Substring(0, 2)}.{Value.Substring(2, 3)}.{Value.Substring(5, 3)}-{Value.Substring(8, 3)}";
        }

        /// <summary>
        /// Additional validation: checks if the TaxId is valid (modulo 11 algorithm).
        /// Note: Simplified implementation, adjust according to country.
        /// </summary>
        public bool IsValid()
        {
            // Implement country-specific validation algorithm
            // For now we only validate that it has 11 digits
            return Value.Length == 11 && Value.All(char.IsDigit);
        }

        #region Equality

        public bool Equals(TaxId? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is TaxId taxId && Equals(taxId);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(TaxId? left, TaxId? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(TaxId? left, TaxId? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from TaxId to string.
        /// </summary>
        public static implicit operator string(TaxId taxId) => taxId.Value;
    }
}
