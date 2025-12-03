namespace SupplierGuard.Application.Screening.DTOs
{
    /// <summary>
    /// DTO to request a screening.
    /// </summary>
    public class ScreeningRequestDto
    {
        /// <summary>
        /// ID of the supplier to be evaluated.
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Sources to consult (1=OffshoreLeaks, 2=WorldBank, 3=OFAC).
        /// </summary>
        public List<int> Sources { get; set; } = new();
    }

    /// <summary>
    /// DTO of the screening result.
    /// </summary>
    public class ScreeningResultDto
    {
        /// <summary>
        /// ID of the evaluated supplier.
        /// </summary>
        public Guid SupplierId { get; set; }

        /// <summary>
        /// Legal name of the evaluated supplier.
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// Entity sought in the lists.
        /// </summary>
        public string SearchedEntity { get; set; } = string.Empty;

        /// <summary>
        /// Total number of matches found.
        /// </summary>
        public int TotalHits { get; set; }

        /// <summary>
        /// List of matches found.
        /// </summary>
        public List<ScreeningHitDto> Hits { get; set; } = new();

        /// <summary>
        /// Date and time of the search.
        /// </summary>
        public DateTime SearchedAt { get; set; }

        /// <summary>
        /// Execution time in seconds.
        /// </summary>
        public double ExecutionTimeSeconds { get; set; }

        /// <summary>
        /// Errors that occurred during the search.
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Indicates if the provider has a high risk (1+ matches).
        /// </summary>
        public bool IsHighRisk => TotalHits > 0;
    }

    /// <summary>
    /// DTO of an individual screening match.
    /// </summary>
    public class ScreeningHitDto
    {
        /// <summary>
        /// Name of the entity found.
        /// </summary>
        public string EntityName { get; set; } = string.Empty;

        /// <summary>
        /// Fuente de la coincidencia (OFAC, WorldBank, OffshoreLeaks).
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Specific attributes of the source.
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new();

        /// <summary>
        /// Match score (0-100).
        /// </summary>
        public double? MatchScore { get; set; }
    }
}
