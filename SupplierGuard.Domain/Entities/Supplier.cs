using SupplierGuard.Domain.Common;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.Exceptions;
using SupplierGuard.Domain.ValueObjects;

namespace SupplierGuard.Domain.Entities
{
    /// <summary>
    /// Supplier Entity (Aggregate Root).
    /// Represent a supplier with all their identification and contact information.
    /// </summary>
    public sealed class Supplier : AuditableEntity
    {
        // Properties using Value Objects
        private CompanyName _legalName = null!;
        private CompanyName _commercialName = null!;
        private TaxId _taxId = null!;
        private PhoneNumber _phoneNumber = null!;
        private Email _email = null!;
        private Website? _website;
        private Address _physicalAddress = null!;
        private AnnualRevenue _annualRevenue = null!;

        /// <summary>
        /// Company Name (Legal name of the company).
        /// </summary>
        public CompanyName LegalName
        {
            get => _legalName;
            private set => _legalName = value ?? throw new ArgumentNullException(nameof(LegalName));
        }

        /// <summary>
        /// Company Trade Name.
        /// </summary>
        public CompanyName CommercialName
        {
            get => _commercialName;
            private set => _commercialName = value ?? throw new ArgumentNullException(nameof(CommercialName));
        }

        /// <summary>
        /// RUT/NIT (Tax Identification) - 11 digits.
        /// </summary>
        public TaxId TaxId
        {
            get => _taxId;
            private set => _taxId = value ?? throw new ArgumentNullException(nameof(TaxId));
        }

        /// <summary>
        /// Phone number.
        /// </summary>
        public PhoneNumber PhoneNumber
        {
            get => _phoneNumber;
            private set => _phoneNumber = value ?? throw new ArgumentNullException(nameof(PhoneNumber));
        }

        /// <summary>
        /// Email address.
        /// </summary>
        public Email Email
        {
            get => _email;
            private set => _email = value ?? throw new ArgumentNullException(nameof(Email));
        }

        /// <summary>
        /// Website (optional).
        /// </summary>
        public Website? Website
        {
            get => _website;
            private set => _website = value;
        }

        /// <summary>
        /// Physical address.
        /// </summary>
        public Address PhysicalAddress
        {
            get => _physicalAddress;
            private set => _physicalAddress = value ?? throw new ArgumentNullException(nameof(PhysicalAddress));
        }

        /// <summary>
        /// Supplier country.
        /// </summary>
        public Country Country { get; private set; }

        /// <summary>
        /// Annual billing in dollars.
        /// </summary>
        public AnnualRevenue AnnualRevenue
        {
            get => _annualRevenue;
            private set => _annualRevenue = value ?? throw new ArgumentNullException(nameof(AnnualRevenue));
        }

        /// <summary>
        /// Private constructor for Entity Framework.
        /// </summary>
        private Supplier() : base()
        {
        }

        /// <summary>
        /// Private constructor for rebuilding from the database.
        /// </summary>
        private Supplier(Guid id) : base(id)
        {
        }

        /// <summary>
        /// Factory method to create a new Supplier.
        /// </summary>
        public static Supplier Create(
            string legalName,
            string commercialName,
            string taxId,
            string phoneNumber,
            string email,
            string? website,
            string physicalAddress,
            Country country,
            decimal annualRevenue)
        {
            var supplier = new Supplier
            {
                LegalName = CompanyName.Create(legalName),
                CommercialName = CompanyName.Create(commercialName),
                TaxId = TaxId.Create(taxId),
                PhoneNumber = PhoneNumber.Create(phoneNumber),
                Email = Email.Create(email),
                Website = Website.CreateOptional(website),
                PhysicalAddress = Address.Create(physicalAddress),
                Country = country,
                AnnualRevenue = AnnualRevenue.Create(annualRevenue)
            };

            supplier.CreatedAt = DateTime.UtcNow;

            return supplier;
        }

        /// <summary>
        /// Update the supplier's information.
        /// </summary>
        public void Update(
            string legalName,
            string commercialName,
            string taxId,
            string phoneNumber,
            string email,
            string? website,
            string physicalAddress,
            Country country,
            decimal annualRevenue)
        {
            LegalName = CompanyName.Create(legalName);
            CommercialName = CompanyName.Create(commercialName);
            TaxId = TaxId.Create(taxId);
            PhoneNumber = PhoneNumber.Create(phoneNumber);
            Email = Email.Create(email);
            Website = Website.CreateOptional(website);
            PhysicalAddress = Address.Create(physicalAddress);
            Country = country;
            AnnualRevenue = AnnualRevenue.Create(annualRevenue);

            LastModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Actualiza solo la información de contacto.
        /// </summary>
        public void UpdateContactInfo(string phoneNumber, string email, string? website)
        {
            PhoneNumber = PhoneNumber.Create(phoneNumber);
            Email = Email.Create(email);
            Website = Website.CreateOptional(website);

            LastModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Update only the annual billing.
        /// </summary>
        public void UpdateAnnualRevenue(decimal amount)
        {
            AnnualRevenue = AnnualRevenue.Create(amount);
            LastModifiedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if the supplier has high billing (> 10 million).
        /// </summary>
        public bool IsHighRevenueSupplier()
        {
            return AnnualRevenue.IsHighRevenue();
        }

        /// <summary>
        /// Gets the name for screening search (uses Company Name by default).
        /// </summary>
        public string GetScreeningName()
        {
            return LegalName.Value;
        }

        /// <summary>
        /// Verify that all supplier data is consistent.
        /// </summary>
        public void Validate()
        {
            if (LegalName == null)
                throw new InvalidSupplierDataException(nameof(LegalName), "Legal name is required.");

            if (CommercialName == null)
                throw new InvalidSupplierDataException(nameof(CommercialName), "Commercial name is required.");

            if (TaxId == null)
                throw new InvalidSupplierDataException(nameof(TaxId), "Tax ID is required.");

            if (!TaxId.IsValid())
                throw new InvalidSupplierDataException(nameof(TaxId), "Tax ID format is invalid.");

            if (PhoneNumber == null)
                throw new InvalidSupplierDataException(nameof(PhoneNumber), "Phone number is required.");

            if (Email == null)
                throw new InvalidSupplierDataException(nameof(Email), "Email is required.");

            if (PhysicalAddress == null)
                throw new InvalidSupplierDataException(nameof(PhysicalAddress), "Physical address is required.");

            if (AnnualRevenue == null)
                throw new InvalidSupplierDataException(nameof(AnnualRevenue), "Annual revenue is required.");
        }

        /// <summary>
        /// String representation of the provider.
        /// </summary>
        public override string ToString()
        {
            return $"{LegalName.Value} ({TaxId.Value})";
        }
    }
}
