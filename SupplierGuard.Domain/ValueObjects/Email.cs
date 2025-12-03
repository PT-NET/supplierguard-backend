using System.Text.RegularExpressions;

namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a valid email address.
    /// </summary>
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Email address.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated Email.
        /// </summary>
        /// <param name="value">Email address.</param>
        /// <returns>Email instance.</returns>
        /// <exception cref="ArgumentException">If the email is not valid.</exception>
        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty.", nameof(value));

            value = value.Trim().ToLowerInvariant();

            if (value.Length > 100)
                throw new ArgumentException("Email cannot exceed 100 characters.", nameof(value));

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format.", nameof(value));

            return new Email(value);
        }

        /// <summary>
        /// Gets the email domain (part after the @).
        /// </summary>
        public string GetDomain()
        {
            var atIndex = Value.IndexOf('@');
            return atIndex >= 0 ? Value.Substring(atIndex + 1) : string.Empty;
        }

        /// <summary>
        /// Gets the email local part (part before the @).
        /// </summary>
        public string GetLocalPart()
        {
            var atIndex = Value.IndexOf('@');
            return atIndex >= 0 ? Value.Substring(0, atIndex) : Value;
        }

        #region Equality

        public bool Equals(Email? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is Email email && Equals(email);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Email? left, Email? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Email? left, Email? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from Email to string.
        /// </summary>
        public static implicit operator string(Email email) => email.Value;
    }
}
