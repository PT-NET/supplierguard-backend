using AutoMapper;
using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Queries.GetSupplierById
{
    /// <summary>
    /// Query to obtain a supplier by their ID.
    /// </summary>
    public record GetSupplierByIdQuery : IRequest<SupplierDto>
    {
        public Guid Id { get; init; }
    }

    /// <summary>
    /// Handler for GetSupplierByIdQuery.
    /// </summary>
    public class GetSupplierByIdQueryHandler : IRequestHandler<GetSupplierByIdQuery, SupplierDto>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public GetSupplierByIdQueryHandler(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SupplierDto> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (supplier == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Supplier), request.Id);
            }

            return _mapper.Map<SupplierDto>(supplier);
        }
    }

    /// <summary>
    /// Validator for GetSupplierByIdQuery.
    /// </summary>
    public class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
    {
        public GetSupplierByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Supplier ID is required.");
        }
    }
}
