using SupplierGuard.Domain.Enums;

namespace SupplierGuard.Application.Suppliers.DTOs
{
    /// <summary>
    /// Full DTO of a supplier (for detailed reading).
    /// </summary>
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string CommercialName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string PhysicalAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public Country CountryCode { get; set; }
        public decimal AnnualRevenue { get; set; }
        public string AnnualRevenueFormatted { get; set; } = string.Empty;
        public bool IsHighRevenue { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// Simplified DTO for supplier listings.
    /// </summary>
    public class SupplierListDto
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string CommercialName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AnnualRevenue { get; set; }
        public string AnnualRevenueFormatted { get; set; } = string.Empty;
        public DateTime? LastModifiedAt { get; set; }
    }

    /// <summary>
    /// DTO to create a supplier.
    /// </summary>
    public class CreateSupplierDto
    {
        public string LegalName { get; set; } = string.Empty;
        public string CommercialName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string PhysicalAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AnnualRevenue { get; set; }
    }

    /// <summary>
    /// DTO to update a supplier.
    /// </summary>
    public class UpdateSupplierDto
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string CommercialName { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string PhysicalAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal AnnualRevenue { get; set; }
    }
}
