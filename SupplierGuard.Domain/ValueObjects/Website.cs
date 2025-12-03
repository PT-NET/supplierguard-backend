namespace SupplierGuard.Domain.ValueObjects
{
    /// <summary>
    /// Value Object that represents a valid website URL.
    /// </summary>
    public sealed class Website : IEquatable<Website>
    {
        /// <summary>
        /// Website URL.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Validated URI of the website.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// Private constructor to ensure validation.
        /// </summary>
        private Website(string value, Uri uri)
        {
            Value = value;
            Uri = uri;
        }

        /// <summary>
        /// Factory method to create a validated Website.
        /// </summary>
        /// <param name="value">Website URL.</param>
        /// <returns>Website instance.</returns>
        /// <exception cref="ArgumentException">If the URL is not valid.</exception>
        public static Website Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Website URL cannot be empty.", nameof(value));

            value = value.Trim();

            // Add https:// if it doesn't have a protocol
            if (!value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                value = $"https://{value}";
            }

            if (value.Length > 255)
                throw new ArgumentException("Website URL cannot exceed 255 characters.", nameof(value));

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                throw new ArgumentException("Invalid website URL format.", nameof(value));

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Website URL must use HTTP or HTTPS protocol.", nameof(value));

            return new Website(value, uri);
        }

        /// <summary>
        /// Creates an optional Website (can be null if the value is empty).
        /// </summary>
        public static Website? CreateOptional(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return Create(value);
        }

        /// <summary>
        /// Gets the website domain (without protocol).
        /// </summary>
        public string GetDomain()
        {
            return Uri.Host;
        }

        /// <summary>
        /// Indicates if the site uses HTTPS.
        /// </summary>
        public bool IsSecure()
        {
            return Uri.Scheme == Uri.UriSchemeHttps;
        }

        #region Equality

        public bool Equals(Website? other)
        {
            if (other is null) return false;
            return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            return obj is Website website && Equals(website);
        }

        public override int GetHashCode()
        {
            return Value.ToLowerInvariant().GetHashCode();
        }

        public static bool operator ==(Website? left, Website? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Website? left, Website? right)
        {
            return !(left == right);
        }

        #endregion

        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from Website to string.
        /// </summary>
        public static implicit operator string(Website website) => website.Value;
    }
}
