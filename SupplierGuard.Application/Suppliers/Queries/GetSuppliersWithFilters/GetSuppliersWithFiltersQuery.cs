using AutoMapper;
using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Models;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Enums;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Queries.GetSuppliersWithFilters
{
    /// <summary>
    /// Query to obtain suppliers with filters, sorting and pagination.
    /// </summary>
    public record GetSuppliersWithFiltersQuery : IRequest<PaginatedList<SupplierListDto>>
    {
        /// <summary>
        /// Search term (search in legal name, commercial name, taxId, email).
        /// </summary>
        public string? SearchTerm { get; init; }

        /// <summary>
        /// Filter by country (optional).
        /// </summary>
        public string? Country { get; init; }

        /// <summary>
        /// Minimum billing (optional).
        /// </summary>
        public decimal? MinRevenue { get; init; }

        /// <summary>
        /// Maximum billing (optional).
        /// </summary>
        public decimal? MaxRevenue { get; init; }

        /// <summary>
        /// Field to sort by (default: LastModifiedAt).
        /// Valid values: LegalName, CommercialName, TaxId, Country, AnnualRevenue, CreatedAt, LastModifiedAt
        /// </summary>
        public string OrderBy { get; init; } = "LastModifiedAt";

        /// <summary>
        /// Ascending (true) or descending (false) order.
        /// </summary>
        public bool Ascending { get; init; } = false;

        /// <summary>
        /// Page number (starting at 1).
        /// </summary>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// Page size (maximum 100).
        /// </summary>
        public int PageSize { get; init; } = 10;
    }

    /// <summary>
    /// Handler for GetSuppliersWithFiltersQuery.
    /// </summary>
    public class GetSuppliersWithFiltersQueryHandler
        : IRequestHandler<GetSuppliersWithFiltersQuery, PaginatedList<SupplierListDto>>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public GetSuppliersWithFiltersQueryHandler(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<SupplierListDto>> Handle(
            GetSuppliersWithFiltersQuery request,
            CancellationToken cancellationToken)
        {
            Country? country = null;
            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                country = CountryExtensions.Parse(request.Country);
            }

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                var allFiltered = await _repository.GetFilteredAsync(
                    searchTerm: request.SearchTerm,
                    country: country,
                    minRevenue: request.MinRevenue,
                    maxRevenue: request.MaxRevenue,
                    orderBy: request.OrderBy,
                    ascending: request.Ascending,
                    cancellationToken: cancellationToken
                );

                var totalCount = allFiltered.Count;
                var pageSize = Math.Min(request.PageSize, 100); 
                var skip = (request.PageNumber - 1) * pageSize;

                var items = allFiltered
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                var dtos = _mapper.Map<List<SupplierListDto>>(items);

                return PaginatedList<SupplierListDto>.Create(
                    dtos,
                    totalCount,
                    request.PageNumber,
                    pageSize
                );
            }

            var suppliers = await _repository.GetFilteredAsync(
                searchTerm: request.SearchTerm,
                country: country,
                minRevenue: request.MinRevenue,
                maxRevenue: request.MaxRevenue,
                orderBy: request.OrderBy,
                ascending: request.Ascending,
                cancellationToken: cancellationToken
            );

            var supplierDtos = _mapper.Map<List<SupplierListDto>>(suppliers);

            return PaginatedList<SupplierListDto>.Create(
                supplierDtos,
                supplierDtos.Count,
                1,
                supplierDtos.Count
            );
        }
    }

    /// <summary>
    /// Validator for GetSuppliersWithFiltersQuery.
    /// </summary>
    public class GetSuppliersWithFiltersQueryValidator : AbstractValidator<GetSuppliersWithFiltersQuery>
    {
        public GetSuppliersWithFiltersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.")
                .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100.");

            RuleFor(x => x.MinRevenue)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum revenue cannot be negative.")
                .When(x => x.MinRevenue.HasValue);

            RuleFor(x => x.MaxRevenue)
                .GreaterThanOrEqualTo(0).WithMessage("Maximum revenue cannot be negative.")
                .When(x => x.MaxRevenue.HasValue);

            RuleFor(x => x)
                .Must(x => x.MinRevenue <= x.MaxRevenue)
                .WithMessage("Minimum revenue cannot be greater than maximum revenue.")
                .When(x => x.MinRevenue.HasValue && x.MaxRevenue.HasValue);

            RuleFor(x => x.OrderBy)
                .Must(BeValidOrderByField)
                .WithMessage("Invalid order by field. Valid values: LegalName, CommercialName, TaxId, Country, AnnualRevenue, CreatedAt, LastModifiedAt")
                .When(x => !string.IsNullOrWhiteSpace(x.OrderBy));
        }

        private bool BeValidOrderByField(string orderBy)
        {
            var validFields = new[]
            {
            "LegalName", "CommercialName", "TaxId", "Country",
            "AnnualRevenue", "CreatedAt", "LastModifiedAt"
        };

            return validFields.Contains(orderBy, StringComparer.OrdinalIgnoreCase);
        }
    }
}
