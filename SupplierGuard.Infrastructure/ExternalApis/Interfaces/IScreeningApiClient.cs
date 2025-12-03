using SupplierGuard.Infrastructure.ExternalApis.Models;

namespace SupplierGuard.Infrastructure.ExternalApis.Interfaces
{
    /// <summary>
    /// Interface for the Screening API client (Exercise 1).
    /// </summary>
    public interface IScreeningApiClient
    {
        /// <summary>
        /// Performs a screening of an entity in the specified sources.
        /// </summary>
        /// <param name="entityName">Name of the entity to search for.</param>
        /// <param name="sources">Sources to search in (1=OFAC, 2=WorldBank, 3=OffshoreLeaks).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Screening result.</returns>
        Task<ScreeningApiResponse> ScreenAsync(
            string entityName,
            List<int> sources,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks the health status of the Screening API.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the API is available, false otherwise.</returns>
        Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
    }

}
