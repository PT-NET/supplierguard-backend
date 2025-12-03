using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;

namespace SupplierGuard.Domain.Repositories
{
    /// <summary>
    /// Supplier repository interface.
    /// Define the data access operations for the Supplier entity.
    /// </summary>
    public interface ISupplierRepository
    {
        /// <summary>
        /// Get a supplier by their ID.
        /// </summary>
        /// <param name="id">Supplier ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The supplier if exists, otherwise null.</returns>
        Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a supplier by their Tax ID (RUT/NIT).
        /// </summary>
        /// <param name="taxId">Supplier's Tax ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The supplier if exists, otherwise null.</returns>
        Task<Supplier?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all suppliers.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all suppliers.</returns>
        Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get suppliers with pagination.
        /// </summary>
        /// <param name="pageNumber">Page number (starting at 1).</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Paginated list of suppliers.</returns>
        Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets filtered and sorted suppliers.
        /// </summary>
        /// <param name="searchTerm">Search term (search in legal name, business name, taxId).</param>
        /// <param name="country">Filter by country (optional).</param>
        /// <param name="minRevenue">Minimum revenue (optional).</param>
        /// <param name="maxRevenue">Maximum revenue (optional).</param>
        /// <param name="orderBy">Sort field.</param>
        /// <param name="ascending">Ascending order (true) or descending (false).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Filtered list of suppliers.</returns>
        Task<List<Supplier>> GetFilteredAsync(
            string? searchTerm = null,
            Country? country = null,
            decimal? minRevenue = null,
            decimal? maxRevenue = null,
            string orderBy = "LastModifiedAt",
            bool ascending = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new supplier.
        /// </summary>
        /// <param name="supplier">Supplier to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="supplier">Supplier to update.</param>
        void Update(Supplier supplier);

        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        /// <param name="supplier">Supplier to delete.</param>
        void Delete(Supplier supplier);

        /// <summary>
        /// Checks if a supplier with the specified Tax ID exists.
        /// </summary>
        /// <param name="taxId">Tax ID to verify.</param>
        /// <param name="excludeId">Supplier ID to exclude from search (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if exists, false otherwise.</returns>
        Task<bool> ExistsByTaxIdAsync(string taxId, Guid? excludeId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total count of suppliers.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Total number of suppliers.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets suppliers by country.
        /// </summary>
        /// <param name="country">Country to filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of suppliers from the specified country.</returns>
        Task<List<Supplier>> GetByCountryAsync(Country country, CancellationToken cancellationToken = default);
    }
}
