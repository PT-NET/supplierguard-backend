using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SupplierGuard.Infrastructure.ExternalApis
{
    /// <summary>
    /// DelegatingHandler to automatically add Auth0 tokens (M2M)
    /// to HTTP requests towards protected APIs.
    /// </summary>
    public class Auth0TokenHandler : DelegatingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Auth0TokenHandler> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiration = DateTime.MinValue;
        private readonly SemaphoreSlim _tokenLock = new(1, 1);

        public Auth0TokenHandler(
            IConfiguration configuration,
            ILogger<Auth0TokenHandler> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Gets an Access Token from Auth0 using Client Credentials Flow (M2M).
        /// Caches the token until 5 minutes before expiration.
        /// </summary>
        private async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken)
        {
            // If token is cached and still valid, return it
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
            {
                _logger.LogDebug("Using cached Auth0 token");
                return _cachedToken;
            }

            // Lock to avoid multiple simultaneous requests to Auth0 token endpoint
            await _tokenLock.WaitAsync(cancellationToken);

            try
            {
                // Double-check after lock
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiration)
                {
                    return _cachedToken;
                }

                _logger.LogInformation("Requesting new Auth0 access token for Screening API");

                var domain = _configuration["ExternalApis:ScreeningApi:Auth0:Domain"]
                    ?? throw new InvalidOperationException("Auth0 Domain not configured");

                var clientId = _configuration["ExternalApis:ScreeningApi:Auth0:ClientId"]
                    ?? throw new InvalidOperationException("Auth0 ClientId not configured");

                var clientSecret = _configuration["ExternalApis:ScreeningApi:Auth0:ClientSecret"]
                    ?? throw new InvalidOperationException("Auth0 ClientSecret not configured");

                var audience = _configuration["ExternalApis:ScreeningApi:Auth0:Audience"]
                    ?? throw new InvalidOperationException("Auth0 Audience not configured");

                using var httpClient = new HttpClient();
                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"https://{domain}/oauth/token");

                var requestBody = new
                {
                    client_id = clientId,
                    client_secret = clientSecret,
                    audience = audience,
                    grant_type = "client_credentials"
                };

                tokenRequest.Content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await httpClient.SendAsync(tokenRequest, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError(
                        "Failed to obtain Auth0 token. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode,
                        errorContent);

                    throw new InvalidOperationException(
                        $"Failed to obtain Auth0 access token. Status: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var tokenResponse = JsonSerializer.Deserialize<Auth0TokenResponse>(responseContent);

                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
                {
                    throw new InvalidOperationException("Invalid Auth0 token response");
                }

                // Cache the token (renew 5 minutes before expiration)
                _cachedToken = tokenResponse.access_token;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in - 300);

                _logger.LogInformation(
                    "Auth0 access token obtained successfully. Expires in {ExpiresIn} seconds",
                    tokenResponse.expires_in);

                return _cachedToken;
            }
            finally
            {
                _tokenLock.Release();
            }
        }

        /// <summary>
        /// Model to deserialize the Auth0 token endpoint response.
        /// </summary>
        private class Auth0TokenResponse
        {
            public string access_token { get; set; } = string.Empty;
            public int expires_in { get; set; }
            public string token_type { get; set; } = string.Empty;
        }
    }
}
