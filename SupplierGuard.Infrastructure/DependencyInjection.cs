using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SupplierGuard.Domain.Interfaces;
using SupplierGuard.Domain.Repositories;
using SupplierGuard.Infrastructure.ExternalApis;
using SupplierGuard.Infrastructure.ExternalApis.Interfaces;
using SupplierGuard.Infrastructure.Persistence;
using SupplierGuard.Infrastructure.Repositories;
using SupplierGuard.Infrastructure.Services;

namespace SupplierGuard.Infrastructure
{
    /// <summary>
    /// Extension class to register Infrastructure layer services.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all Infrastructure layer services.
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ===== DATABASE =====
            AddDatabase(services, configuration);

            // ===== REPOSITORIES =====
            AddRepositories(services);

            // ===== SERVICES =====
            AddServices(services);

            // ===== EXTERNAL APIS =====
            AddExternalApis(services, configuration);

            return services;
        }

        /// <summary>
        /// Registers the DbContext and database configuration.
        /// </summary>
        private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        // Retry policy to handle transient SQL Server failures
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);

                        sqlOptions.CommandTimeout(60);
                        sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    });

                // Additional configurations for development
#if DEBUG
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
#endif
            });

            // Register IApplicationDbContext
            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());
        }

        /// <summary>
        /// Registers repositories.
        /// </summary>
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<ISupplierRepository, SupplierRepository>();
        }

        /// <summary>
        /// Registers internal services.
        /// </summary>
        private static void AddServices(IServiceCollection services)
        {
            services.AddSingleton<IDateTimeService, DateTimeService>();
        }

        /// <summary>
        /// Registers HTTP clients for external APIs.
        /// </summary>
        private static void AddExternalApis(IServiceCollection services, IConfiguration configuration)
        {
            // Register Auth0TokenHandler
            services.AddTransient<Auth0TokenHandler>();

            // Screening API configuration (Exercise 1)
            var screeningApiSection = configuration.GetSection("ExternalApis:ScreeningApi");
            var baseUrl = screeningApiSection["BaseUrl"];
            var timeoutSeconds = screeningApiSection.GetValue<int>("TimeoutSeconds", 60);
            var retryCount = screeningApiSection.GetValue<int>("RetryCount", 3);

            if (!string.IsNullOrEmpty(baseUrl))
            {
                services.AddHttpClient<IScreeningApiClient, ScreeningApiClient>(client =>
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "SupplierGuard/1.0");
                })
                .AddHttpMessageHandler<Auth0TokenHandler>()
                .AddPolicyHandler(GetRetryPolicy(retryCount))
                .AddPolicyHandler(GetCircuitBreakerPolicy());
            }
        }

        /// <summary>
        /// Retry policy with exponential backoff.
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError() // 5xx and 408
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // 429
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Backoff: 2, 4, 8 seconds
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to: {outcome.Result?.StatusCode}");
                    });
        }

        /// <summary>
        /// Circuit breaker policy to prevent cascade failures.
        /// </summary>
        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5, // Opens circuit after 5 failures
                    durationOfBreak: TimeSpan.FromSeconds(30), // Keeps circuit open for 30 seconds
                    onBreak: (outcome, duration) =>
                    {
                        Console.WriteLine($"Circuit breaker opened for {duration.TotalSeconds}s");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit breaker reset");
                    });
        }
    }
}
