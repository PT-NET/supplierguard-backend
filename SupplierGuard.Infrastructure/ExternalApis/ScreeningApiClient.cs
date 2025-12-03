using Microsoft.Extensions.Logging;
using SupplierGuard.Infrastructure.ExternalApis.Interfaces;
using SupplierGuard.Infrastructure.ExternalApis.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace SupplierGuard.Infrastructure.ExternalApis
{
    /// <summary>
    /// HTTP client to communicate with the Screening API (Exercise 1).
    /// Includes error handling, retry policies and logging.
    /// </summary>
    public class ScreeningApiClient : IScreeningApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScreeningApiClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ScreeningApiClient(HttpClient httpClient, ILogger<ScreeningApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Performs a screening of an entity against the specified sources.
        /// </summary>
        public async Task<ScreeningApiResponse> ScreenAsync(
            string entityName,
            List<int> sources,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Entity name cannot be empty.", nameof(entityName));

            if (sources == null || !sources.Any())
                throw new ArgumentException("At least one source must be specified.", nameof(sources));

            _logger.LogInformation(
                "Starting screening for entity: {EntityName} with sources: {Sources}",
                entityName,
                string.Join(", ", sources));

            try
            {
                var request = new ScreeningApiRequest
                {
                    EntityName = entityName,
                    Sources = sources
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "/api/screening/screen",
                    request,
                    _jsonOptions,
                    cancellationToken);

                // Handle successful response
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ScreeningApiResponse>(
                        _jsonOptions,
                        cancellationToken);

                    if (result == null)
                    {
                        _logger.LogError("Received null response from Screening API");
                        throw new InvalidOperationException("Screening API returned null response");
                    }

                    _logger.LogInformation(
                        "Screening completed successfully for {EntityName}. Total hits: {TotalHits}",
                        entityName,
                        result.TotalHits);

                    return result;
                }

                // Handle rate limiting (429)
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 60;

                    _logger.LogWarning(
                        "Rate limit exceeded for Screening API. Retry after {RetryAfter} seconds",
                        retryAfter);

                    var errorResponse = await response.Content.ReadFromJsonAsync<ScreeningApiErrorResponse>(
                        _jsonOptions,
                        cancellationToken);

                    throw new ScreeningApiException(
                        $"Rate limit exceeded. Retry after {retryAfter} seconds.",
                        (int)response.StatusCode,
                        errorResponse);
                }

                // Handle other errors
                var errorContent = await response.Content.ReadFromJsonAsync<ScreeningApiErrorResponse>(
                    _jsonOptions,
                    cancellationToken);

                _logger.LogError(
                    "Screening API returned error status {StatusCode}: {Message}",
                    response.StatusCode,
                    errorContent?.Message ?? "Unknown error");

                throw new ScreeningApiException(
                    errorContent?.Message ?? $"Screening API returned status code {response.StatusCode}",
                    (int)response.StatusCode,
                    errorContent);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while calling Screening API");
                throw new ScreeningApiException("Failed to connect to Screening API", 0, null, ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Screening API request timed out");
                throw new ScreeningApiException("Screening API request timed out", 0, null, ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize Screening API response");
                throw new ScreeningApiException("Invalid response from Screening API", 0, null, ex);
            }
        }

        /// <summary>
        /// Checks the health status of the Screening API.
        /// </summary>
        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Checking Screening API health");

                var response = await _httpClient.GetAsync(
                    "/api/screening/health",
                    cancellationToken);

                var isHealthy = response.IsSuccessStatusCode;

                _logger.LogInformation(
                    "Screening API health check result: {IsHealthy}",
                    isHealthy ? "Healthy" : "Unhealthy");

                return isHealthy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform health check on Screening API");
                return false;
            }
        }
    }

    /// <summary>
    /// Specific exception for Screening API errors.
    /// </summary>
    public class ScreeningApiException : Exception
    {
        /// <summary>
        /// HTTP status code of the error.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// API error details.
        /// </summary>
        public ScreeningApiErrorResponse? ErrorResponse { get; }

        public ScreeningApiException(
            string message,
            int statusCode,
            ScreeningApiErrorResponse? errorResponse,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorResponse = errorResponse;
        }

        /// <summary>
        /// Indicates if the error is a rate limit error.
        /// </summary>
        public bool IsRateLimitError()
        {
            return StatusCode == 429;
        }

        /// <summary>
        /// Gets the seconds to retry (in case of rate limit).
        /// </summary>
        public int? GetRetryAfterSeconds()
        {
            return ErrorResponse?.Errors?.FirstOrDefault()?.RetryAfter;
        }
    }
}
