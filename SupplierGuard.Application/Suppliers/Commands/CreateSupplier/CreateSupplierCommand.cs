using AutoMapper;
using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Commands.CreateSupplier
{
    /// <summary>
    /// Command for create a new supplier.
    /// </summary>
    public record CreateSupplierCommand : IRequest<SupplierDto>
    {
        public string LegalName { get; init; } = string.Empty;
        public string CommercialName { get; init; } = string.Empty;
        public string TaxId { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string? Website { get; init; }
        public string PhysicalAddress { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public decimal AnnualRevenue { get; init; }
    }

    /// <summary>
    /// Handler for CreateSupplierCommand.
    /// </summary>
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, SupplierDto>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public CreateSupplierCommandHandler(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            
            var exists = await _repository.ExistsByTaxIdAsync(request.TaxId, null, cancellationToken);
            if (exists)
            {
                throw new ConflictException($"A supplier with Tax ID '{request.TaxId}' already exists.");
            }

            
            var country = CountryExtensions.Parse(request.Country);

            
            var supplier = Supplier.Create(
                legalName: request.LegalName,
                commercialName: request.CommercialName,
                taxId: request.TaxId,
                phoneNumber: request.PhoneNumber,
                email: request.Email,
                website: request.Website,
                physicalAddress: request.PhysicalAddress,
                country: country,
                annualRevenue: request.AnnualRevenue
            );

            await _repository.AddAsync(supplier, cancellationToken);

            
            return _mapper.Map<SupplierDto>(supplier);
        }
    }

    /// <summary>
    /// Validator for CreateSupplierCommand.
    /// </summary>
    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            RuleFor(x => x.LegalName)
                .NotEmpty().WithMessage("Legal name is required.")
                .MinimumLength(2).WithMessage("Legal name must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Legal name cannot exceed 200 characters.");

            RuleFor(x => x.CommercialName)
                .NotEmpty().WithMessage("Commercial name is required.")
                .MinimumLength(2).WithMessage("Commercial name must be at least 2 characters.")
                .MaximumLength(200).WithMessage("Commercial name cannot exceed 200 characters.");

            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("Tax ID is required.")
                .Must(BeValidTaxId).WithMessage("Tax ID must be exactly 11 digits.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MinimumLength(7).WithMessage("Phone number must be at least 7 characters.")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

            RuleFor(x => x.Website)
                .MaximumLength(255).WithMessage("Website URL cannot exceed 255 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Website));

            RuleFor(x => x.PhysicalAddress)
                .NotEmpty().WithMessage("Physical address is required.")
                .MinimumLength(5).WithMessage("Address must be at least 5 characters.")
                .MaximumLength(500).WithMessage("Address cannot exceed 500 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .Must(BeValidCountry).WithMessage("Invalid country.");

            RuleFor(x => x.AnnualRevenue)
                .GreaterThanOrEqualTo(0).WithMessage("Annual revenue cannot be negative.")
                .LessThan(1_000_000_000_000).WithMessage("Annual revenue exceeds maximum allowed value.");
        }

        private bool BeValidTaxId(string taxId)
        {
            if (string.IsNullOrWhiteSpace(taxId))
                return false;

            var cleanTaxId = new string(taxId.Where(char.IsDigit).ToArray());
            return cleanTaxId.Length == 11;
        }

        private bool BeValidCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
                return false;

            try
            {
                CountryExtensions.Parse(country);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
