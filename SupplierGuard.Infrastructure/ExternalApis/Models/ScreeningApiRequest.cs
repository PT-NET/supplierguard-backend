namespace SupplierGuard.Infrastructure.ExternalApis.Models
{
    /// <summary>
    /// Request for the Screening API (Exercise 1).
    /// </summary>
    public class ScreeningApiRequest
    {
        /// <summary>
        /// Name of the entity to search for.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Sources to search in (1=OffshoreLeaks, 2=WorldBank, 3=OFAC).
        /// </summary>
        public List<int> Sources { get; set; } = new();
    }

    /// <summary>
    /// Response from the Screening API (Exercise 1).
    /// </summary>
    public class ScreeningApiResponse
    {
        /// <summary>
        /// Searched entity.
        /// </summary>
        public string SearchedEntity { get; set; } = string.Empty;

        /// <summary>
        /// Total number of matches found.
        /// </summary>
        public int TotalHits { get; set; }

        /// <summary>
        /// List of matches.
        /// </summary>
        public List<ScreeningHit> Hits { get; set; } = new();

        /// <summary>
        /// Date and time of the search.
        /// </summary>
        public DateTime SearchedAt { get; set; }

        /// <summary>
        /// Execution time in seconds.
        /// </summary>
        public double ExecutionTimeSeconds { get; set; }

        /// <summary>
        /// Errors occurred during the search.
        /// </summary>
        public List<string>? Errors { get; set; }
    }

    /// <summary>
    /// Represents an individual screening match.
    /// </summary>
    public class ScreeningHit
    {
        /// <summary>
        /// Name of the entity found.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Source of the match (OffshoreLeaks, WorldBank, OFAC).
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Source-specific attributes.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new();

        /// <summary>
        /// Match score (0-100).
        /// </summary>
        public double? MatchScore { get; set; }
    }

    /// <summary>
    /// Error response from the Screening API.
    /// </summary>
    public class ScreeningApiErrorResponse
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional error details.
        /// </summary>
        public List<ErrorDetail>? Errors { get; set; }

        /// <summary>
        /// Error timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Detail of a specific error.
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Field related to the error.
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Seconds to retry (in case of rate limit).
        /// </summary>
        public int? RetryAfter { get; set; }
    }
}
