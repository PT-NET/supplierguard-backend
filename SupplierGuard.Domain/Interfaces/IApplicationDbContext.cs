using Microsoft.EntityFrameworkCore;
using SupplierGuard.Domain.Entities;

namespace SupplierGuard.Domain.Interfaces
{
    /// <summary>
    /// DbContext interface for the Application layer.
    /// Allows you to decouple Application from Infrastructure.
    /// </summary>
    public interface IApplicationDbContext
    {
        /// <summary>
        /// DbSet of suppliers.
        /// </summary>
        DbSet<Supplier> Suppliers { get; }

        /// <summary>
        /// Save the changes to the database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Save changes synchronously.
        /// </summary>
        int SaveChanges();
    }
}
