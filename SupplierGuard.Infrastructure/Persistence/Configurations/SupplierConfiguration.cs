using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.ValueObjects;

namespace SupplierGuard.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the Supplier entity.
    /// Defines the mapping between the domain entity and the database table.
    /// </summary>
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            // Table name
            builder.ToTable("Suppliers");

            // Primary key
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                .ValueGeneratedNever(); // The Guid is generated in the domain

            // ===== VALUE OBJECTS CONFIGURATION =====

            // LegalName (CompanyName)
            builder.Property(s => s.LegalName)
                .HasMaxLength(200)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => CompanyName.Create(v))
                .HasColumnName("LegalName");

            // CommercialName (CompanyName)
            builder.Property(s => s.CommercialName)
                .HasMaxLength(200)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => CompanyName.Create(v))
                .HasColumnName("CommercialName");

            // TaxId (Value Object)
            builder.Property(s => s.TaxId)
                .HasMaxLength(11)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => TaxId.Create(v))
                .HasColumnName("TaxId");

            // PhoneNumber (Value Object)
            builder.Property(s => s.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => PhoneNumber.Create(v))
                .HasColumnName("PhoneNumber");

            // Email (Value Object)
            builder.Property(s => s.Email)
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => Email.Create(v))
                .HasColumnName("Email");

            // Website (Value Object - nullable)
            builder.Property(s => s.Website)
                .HasMaxLength(255)
                .IsRequired(false)
                .HasConversion(
                    v => v != null ? v.Value : null,
                    v => v != null ? Website.Create(v) : null)
                .HasColumnName("Website");

            // PhysicalAddress (Value Object)
            builder.Property(s => s.PhysicalAddress)
                .HasMaxLength(500)
                .IsRequired()
                .HasConversion(
                    v => v.Value,
                    v => Address.Create(v))
                .HasColumnName("PhysicalAddress");

            // Country (Enum)
            builder.Property(s => s.Country)
                .HasMaxLength(100)
                .IsRequired()
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<Country>(v))
                .HasColumnName("Country");

            // AnnualRevenue (Value Object - Money)
            builder.Property(s => s.AnnualRevenue)
                .HasPrecision(18, 2)
                .IsRequired()
                .HasConversion(
                    v => v.Amount,
                    v => AnnualRevenue.Create(v))
                .HasColumnName("AnnualRevenue");

            // ===== AUDIT PROPERTIES =====

            builder.Property(s => s.CreatedAt)
                .IsRequired()
                .HasColumnName("CreatedAt");

            builder.Property(s => s.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("CreatedBy");

            builder.Property(s => s.LastModifiedAt)
                .HasColumnName("LastModifiedAt");

            builder.Property(s => s.LastModifiedBy)
                .HasMaxLength(100)
                .HasColumnName("LastModifiedBy");

            // ===== INDEXES =====

            // Unique index on TaxId (cannot have duplicate RUTs)
            builder.HasIndex(s => s.TaxId)
                .IsUnique()
                .HasDatabaseName("IX_Suppliers_TaxId");

            // Index on LegalName for searches
            builder.HasIndex(s => s.LegalName)
                .HasDatabaseName("IX_Suppliers_LegalName");

            // Index on LastModifiedAt for sorting (exercise requirement)
            builder.HasIndex(s => s.LastModifiedAt)
                .HasDatabaseName("IX_Suppliers_LastModifiedAt");

            // Index on Country for filters
            builder.HasIndex(s => s.Country)
                .HasDatabaseName("IX_Suppliers_Country");

            // Composite index for email searches
            builder.HasIndex(s => s.Email)
                .HasDatabaseName("IX_Suppliers_Email");

            // ===== ADDITIONAL CONSTRAINTS =====

            // Check constraint for AnnualRevenue >= 0
            builder.ToTable(t => t.HasCheckConstraint(
                "CK_Suppliers_AnnualRevenue",
                "[AnnualRevenue] >= 0"));
        }
    }
}
