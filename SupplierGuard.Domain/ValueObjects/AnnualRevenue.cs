namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents the annual revenue in US dollars (USD).
    /// </summary>
    public sealed class AnnualRevenue : IEquatable<AnnualRevenue>, IComparable<AnnualRevenue>
    {
        /// <summary>
        /// Annual revenue amount in USD.
        /// </summary>
        public decimal Amount { get; }

        /// <summary>
        /// Currency code (always USD).
        /// </summary>
        public string Currency => "USD";

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private AnnualRevenue(decimal amount)
        {
            Amount = amount;
        }

        /// <summary>
        /// Factory method to create a validated AnnualRevenue.
        /// </summary>
        /// <param name="amount">Amount in dollars.</param>
        /// <returns>AnnualRevenue instance.</returns>
        /// <exception cref="ArgumentException">If the amount is not valid.</exception>
        public static AnnualRevenue Create(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Annual revenue cannot be negative.", nameof(amount));

            if (amount > 999_999_999_999.99m) // Limit: 1 trillion dollars
                throw new ArgumentException("Annual revenue exceeds maximum allowed value.", nameof(amount));

            // Round to 2 decimals
            amount = Math.Round(amount, 2);

            return new AnnualRevenue(amount);
        }

        /// <summary>
        /// Creates a zero AnnualRevenue (default).
        /// </summary>
        public static AnnualRevenue Zero => new(0);

        /// <summary>
        /// Returns the amount formatted as currency.
        /// Example: $1,234,567.89
        /// </summary>
        public string Formatted()
        {
            return Amount.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
        }

        /// <summary>
        /// Returns the amount in accounting format (without currency symbol).
        /// Example: 1,234,567.89
        /// </summary>
        public string FormattedAccounting()
        {
            return Amount.ToString("N2", System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indicates if the revenue is considered high (more than 10 million).
        /// </summary>
        public bool IsHighRevenue()
        {
            return Amount > 10_000_000m;
        }

        /// <summary>
        /// Indicates if the revenue is zero or very low (less than 1000).
        /// </summary>
        public bool IsLowRevenue()
        {
            return Amount < 1_000m;
        }

        #region Operators

        public static AnnualRevenue operator +(AnnualRevenue left, AnnualRevenue right)
        {
            return Create(left.Amount + right.Amount);
        }

        public static AnnualRevenue operator -(AnnualRevenue left, AnnualRevenue right)
        {
            return Create(left.Amount - right.Amount);
        }

        public static bool operator >(AnnualRevenue left, AnnualRevenue right)
        {
            return left.Amount > right.Amount;
        }

        public static bool operator <(AnnualRevenue left, AnnualRevenue right)
        {
            return left.Amount < right.Amount;
        }

        public static bool operator >=(AnnualRevenue left, AnnualRevenue right)
        {
            return left.Amount >= right.Amount;
        }

        public static bool operator <=(AnnualRevenue left, AnnualRevenue right)
        {
            return left.Amount <= right.Amount;
        }

        #endregion

        #region Equality

        public bool Equals(AnnualRevenue? other)
        {
            if (other is null) return false;
            return Amount == other.Amount;
        }

        public override bool Equals(object? obj)
        {
            return obj is AnnualRevenue revenue && Equals(revenue);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode();
        }

        public static bool operator ==(AnnualRevenue? left, AnnualRevenue? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(AnnualRevenue? left, AnnualRevenue? right)
        {
            return !(left == right);
        }

        #endregion

        #region IComparable

        public int CompareTo(AnnualRevenue? other)
        {
            if (other is null) return 1;
            return Amount.CompareTo(other.Amount);
        }

        #endregion

        public override string ToString() => Formatted();

        /// <summary>
        /// Implicit conversion from AnnualRevenue to decimal.
        /// </summary>
        public static implicit operator decimal(AnnualRevenue revenue) => revenue.Amount;
    }
}
