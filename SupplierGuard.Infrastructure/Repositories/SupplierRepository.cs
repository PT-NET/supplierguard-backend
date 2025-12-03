using Microsoft.EntityFrameworkCore;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.Repositories;
using SupplierGuard.Infrastructure.Persistence;

namespace SupplierGuard.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation of the supplier repository using Entity Framework Core.
    /// </summary>
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets a supplier by their ID.
        /// </summary>
        public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        /// <summary>
        /// Gets a supplier by their Tax ID (RUT/NIT).
        /// </summary>
        public async Task<Supplier?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(taxId))
                return null;

            // Clean the taxId for search (digits only)
            var cleanTaxId = new string(taxId.Where(char.IsDigit).ToArray());

            // ✅ SOLUTION: Load all and filter in memory for Value Objects
            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return suppliers.FirstOrDefault(s => s.TaxId.Value == cleanTaxId);
        }

        /// <summary>
        /// Gets all suppliers ordered by last modification (descending).
        /// </summary>
        public async Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .OrderByDescending(s => s.LastModifiedAt ?? s.CreatedAt) // Order by last edit (requirement)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Gets suppliers with pagination.
        /// </summary>
        public async Task<(List<Supplier> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Maximum limit

            var query = _context.Suppliers.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.LastModifiedAt ?? s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <summary>
        /// Gets filtered and sorted suppliers.
        /// </summary>
        public async Task<List<Supplier>> GetFilteredAsync(
        string? searchTerm = null,
        Country? country = null,
        decimal? minRevenue = null,
        decimal? maxRevenue = null,
        string orderBy = "LastModifiedAt",
        bool ascending = false,
        CancellationToken cancellationToken = default)
        {
            var query = _context.Suppliers.AsNoTracking();

            if (country.HasValue)
            {
                query = query.Where(s => s.Country == country.Value);
            }

            if (minRevenue.HasValue)
            {
                query = query.Where(s => s.AnnualRevenue.Amount >= minRevenue.Value);
            }

            if (maxRevenue.HasValue)
            {
                query = query.Where(s => s.AnnualRevenue.Amount <= maxRevenue.Value);
            }

            query = orderBy.ToLower() switch
            {
                "legalname" => ascending
                    ? query.OrderBy(s => EF.Property<string>(s, "LegalName"))
                    : query.OrderByDescending(s => EF.Property<string>(s, "LegalName")),

                "commercialname" => ascending
                    ? query.OrderBy(s => EF.Property<string>(s, "CommercialName"))
                    : query.OrderByDescending(s => EF.Property<string>(s, "CommercialName")),

                "taxid" => ascending
                    ? query.OrderBy(s => EF.Property<string>(s, "TaxId"))
                    : query.OrderByDescending(s => EF.Property<string>(s, "TaxId")),

                "country" => ascending
                    ? query.OrderBy(s => s.Country)
                    : query.OrderByDescending(s => s.Country),

                "annualrevenue" => ascending
                    ? query.OrderBy(s => EF.Property<decimal>(s, "AnnualRevenue"))
                    : query.OrderByDescending(s => EF.Property<decimal>(s, "AnnualRevenue")),

                "createdat" => ascending
                    ? query.OrderBy(s => s.CreatedAt)
                    : query.OrderByDescending(s => s.CreatedAt),

                "lastmodifiedat" or _ => ascending
                    ? query.OrderBy(s => s.LastModifiedAt ?? s.CreatedAt)
                    : query.OrderByDescending(s => s.LastModifiedAt ?? s.CreatedAt)
            };

            var results = await query.ToListAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                results = results.Where(s =>
                    s.LegalName.Value.ToLower().Contains(searchTerm) ||
                    s.CommercialName.Value.ToLower().Contains(searchTerm) ||
                    s.TaxId.Value.Contains(searchTerm) ||
                    s.Email.Value.ToLower().Contains(searchTerm)
                ).ToList();
            }

            return results;
        }

        /// <summary>
        /// Adds a new supplier.
        /// </summary>
        public async Task AddAsync(Supplier supplier, CancellationToken cancellationToken = default)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            await _context.Suppliers.AddAsync(supplier, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        public void Update(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            _context.Suppliers.Update(supplier);
        }

        /// <summary>
        /// Deletes a supplier.
        /// </summary>
        public void Delete(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            _context.Suppliers.Remove(supplier);
        }

        /// <summary>
        /// Checks if a supplier with the specified Tax ID exists.
        /// </summary>
        public async Task<bool> ExistsByTaxIdAsync(
    string taxId,
    Guid? excludeId = null,
    CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(taxId))
                return false;

            var cleanTaxId = new string(taxId.Where(char.IsDigit).ToArray());

            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (excludeId.HasValue)
            {
                return suppliers.Any(s => s.Id != excludeId.Value && s.TaxId.Value == cleanTaxId);
            }

            return suppliers.Any(s => s.TaxId.Value == cleanTaxId);
        }

        /// <summary>
        /// Gets the total count of suppliers.
        /// </summary>
        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers.CountAsync(cancellationToken);
        }

        /// <summary>
        /// Gets suppliers by country.
        /// </summary>
        public async Task<List<Supplier>> GetByCountryAsync(
            Country country,
            CancellationToken cancellationToken = default)
        {
            return await _context.Suppliers
                .AsNoTracking()
                .Where(s => s.Country == country)
                .OrderByDescending(s => s.LastModifiedAt ?? s.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
