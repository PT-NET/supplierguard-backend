using System.Text.RegularExpressions;

namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a valid phone number.
    /// Accepts different international formats.
    /// </summary>
    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex PhoneRegex = new(
            @"^\+?[1-9]\d{1,14}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Phone number in clean format (only digits and +).
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private PhoneNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a validated PhoneNumber.
        /// </summary>
        /// <param name="value">Phone number.</param>
        /// <returns>PhoneNumber instance.</returns>
        /// <exception cref="ArgumentException">If the phone number is not valid.</exception>
        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Phone number cannot be empty.", nameof(value));

            // Clean: remove spaces, dashes, parentheses
            var cleanValue = new string(value.Where(c => char.IsDigit(c) || c == '+').ToArray());

            if (string.IsNullOrEmpty(cleanValue))
                throw new ArgumentException("Phone number must contain digits.", nameof(value));

            if (cleanValue.Length < 7 || cleanValue.Length > 20)
                throw new ArgumentException("Phone number must be between 7 and 20 characters.", nameof(value));

            if (!PhoneRegex.IsMatch(cleanValue))
                throw new ArgumentException("Invalid phone number format.", nameof(value));

            return new PhoneNumber(cleanValue);
        }

        /// <summary>
        /// Returns the formatted number for display.
        /// Example: +51987654321 → +51 987 654 321
        /// </summary>
        public string Formatted()
        {
            if (Value.StartsWith("+"))
            {
                var countryCode = Value.Substring(0, Math.Min(3, Value.Length));
                var restOfNumber = Value.Substring(countryCode.Length);

                return $"{countryCode} {FormatNumberPart(restOfNumber)}";
            }

            return FormatNumberPart(Value);
        }

        private static string FormatNumberPart(string number)
        {
            if (number.Length <= 3)
                return number;

            // Group in sets of 3 digits
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < number.Length; i++)
            {
                if (i > 0 && i % 3 == 0)
                    result.Append(' ');
                result.Append(number[i]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Indicates if the number has a country code.
        /// </summary>
        public bool HasCountryCode()
        {
            return Value.StartsWith("+");
        }

        #region Equality

        public bool Equals(PhoneNumber? other)
        {
            if (other is null) return false;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is PhoneNumber phone && Equals(phone);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from PhoneNumber to string.
        /// </summary>
        public static implicit operator string(PhoneNumber phone) => phone.Value;
    }
}
