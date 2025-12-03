using Microsoft.EntityFrameworkCore;
using SupplierGuard.Domain.Common;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Interfaces;

namespace SupplierGuard.Infrastructure.Persistence
{
    /// <summary>
    /// Main application DbContext.
    /// Manages persistence of all entities using Entity Framework Core.
    /// </summary>
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;

        /// <summary>
        /// DbSet for suppliers.
        /// </summary>
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        /// <summary>
        /// Model configuration using Fluent API.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all entity configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        /// <summary>
        /// Override SaveChanges for automatic auditing with authenticated user.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Get current user (Auth0 UserId or "system" as fallback)
            var currentUser = _currentUserService.UserId ?? "system";

            // Automatic entity auditing
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUser;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Override synchronous SaveChanges.
        /// </summary>
        public override int SaveChanges()
        {
            // Get current user (Auth0 UserId or "system" as fallback)
            var currentUser = _currentUserService.UserId ?? "system";

            // Automatic entity auditing
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = currentUser;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = currentUser;
                        break;
                }
            }

            return base.SaveChanges();
        }
    }


}
