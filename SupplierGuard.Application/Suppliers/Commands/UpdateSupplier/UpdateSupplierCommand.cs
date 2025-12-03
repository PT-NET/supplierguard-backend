using AutoMapper;
using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Domain.Interfaces;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Commands.UpdateSupplier
{
    /// <summary>
    /// Command to update an existing provider.
    /// </summary>
    public record UpdateSupplierCommand : IRequest<SupplierDto>
    {
        public Guid Id { get; init; }
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
    /// Handler for UpdateSupplierCommand.
    /// </summary>
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, SupplierDto>
    {
        private readonly ISupplierRepository _repository;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdateSupplierCommandHandler(
            ISupplierRepository repository,
            IApplicationDbContext context,
            IMapper mapper)
        {
            _repository = repository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (supplier == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Supplier), request.Id);
            }

            var exists = await _repository.ExistsByTaxIdAsync(request.TaxId, request.Id, cancellationToken);
            if (exists)
            {
                throw new ConflictException($"Another supplier with Tax ID '{request.TaxId}' already exists.");
            }

            var country = CountryExtensions.Parse(request.Country);

            supplier.Update(
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

            _repository.Update(supplier);

            await _context.SaveChangesAsync(cancellationToken);

            return _mapper.Map<SupplierDto>(supplier);
        }
    }

    /// <summary>
    /// Validator for UpdateSupplierCommand.
    /// </summary>
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Supplier ID is required.");

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
