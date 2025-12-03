using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Application.Screening.DTOs;
using SupplierGuard.Domain.Repositories;
using SupplierGuard.Infrastructure.ExternalApis;
using SupplierGuard.Infrastructure.ExternalApis.Interfaces;

namespace SupplierGuard.Application.Screening.Commands.PerformScreening
{
    /// <summary>
    /// Command to perform a supplier screening against high-risk lists.
    /// </summary>
    public record PerformScreeningCommand : IRequest<ScreeningResultDto>
    {
        /// <summary>
        /// ID of the supplier to evaluate.
        /// </summary>
        public Guid SupplierId { get; init; }

        /// <summary>
        /// Sources to query (1=OffshoreLeaks, 2=WorldBank, 3=OFAC).
        /// Minimum 1, maximum 3.
        /// </summary>
        public List<int> Sources { get; init; } = new();
    }

    /// <summary>
    /// Handler for PerformScreeningCommand.
    /// </summary>
    public class PerformScreeningCommandHandler : IRequestHandler<PerformScreeningCommand, ScreeningResultDto>
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IScreeningApiClient _screeningClient;

        public PerformScreeningCommandHandler(
            ISupplierRepository supplierRepository,
            IScreeningApiClient screeningClient)
        {
            _supplierRepository = supplierRepository;
            _screeningClient = screeningClient;
        }

        public async Task<ScreeningResultDto> Handle(
            PerformScreeningCommand request,
            CancellationToken cancellationToken)
        {
            
            var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
            if (supplier == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Supplier), request.SupplierId);
            }

            
            var entityName = supplier.GetScreeningName();

            try
            {
                var apiResponse = await _screeningClient.ScreenAsync(
                    entityName,
                    request.Sources,
                    cancellationToken
                );

                var result = new ScreeningResultDto
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.LegalName.Value,
                    SearchedEntity = apiResponse.SearchedEntity,
                    TotalHits = apiResponse.TotalHits,
                    SearchedAt = apiResponse.SearchedAt,
                    ExecutionTimeSeconds = apiResponse.ExecutionTimeSeconds,
                    Errors = apiResponse.Errors,
                    Hits = apiResponse.Hits.Select(h => new ScreeningHitDto
                    {
                        EntityName = h.EntityName,
                        Source = h.Source,
                        Attributes = h.Attributes,
                        MatchScore = h.MatchScore
                    }).ToList()
                };

                return result;
            }
            catch (ScreeningApiException ex)
            {
                if (ex.IsRateLimitError())
                {
                    var retryAfter = ex.GetRetryAfterSeconds();
                    throw new BadRequestException(
                        $"Screening API rate limit exceeded. Please retry after {retryAfter} seconds.");
                }

                throw new BadRequestException($"Screening API error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Validator for PerformScreeningCommand.
    /// </summary>
    public class PerformScreeningCommandValidator : AbstractValidator<PerformScreeningCommand>
    {
        public PerformScreeningCommandValidator()
        {
            RuleFor(x => x.SupplierId)
                .NotEmpty().WithMessage("Supplier ID is required.");

            RuleFor(x => x.Sources)
                .NotEmpty().WithMessage("At least one source must be specified.")
                .Must(sources => sources.Count >= 1 && sources.Count <= 3)
                .WithMessage("You must select between 1 and 3 sources.");

            RuleFor(x => x.Sources)
                .Must(sources => sources.All(s => s >= 1 && s <= 3))
                .WithMessage("Invalid source. Valid sources are: 1 (OFAC), 2 (WorldBank), 3 (OffshoreLeaks).")
                .When(x => x.Sources != null && x.Sources.Any());

            RuleFor(x => x.Sources)
                .Must(sources => sources.Distinct().Count() == sources.Count)
                .WithMessage("Duplicate sources are not allowed.")
                .When(x => x.Sources != null && x.Sources.Any());
        }
    }

}
