using AutoMapper;
using MediatR;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Queries.GetAllSuppliers
{
    /// <summary>
    /// Query to get all suppliers.
    /// Suppliers are returned sorted by last modification (descending).
    /// </summary>
    public record GetAllSuppliersQuery : IRequest<List<SupplierListDto>>
    {
    }

    /// <summary>
    /// Handler for GetAllSuppliersQuery.
    /// </summary>
    public class GetAllSuppliersQueryHandler : IRequestHandler<GetAllSuppliersQuery, List<SupplierListDto>>
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSuppliersQueryHandler(ISupplierRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<SupplierListDto>> Handle(GetAllSuppliersQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _repository.GetAllAsync(cancellationToken);

            return _mapper.Map<List<SupplierListDto>>(suppliers);
        }
    }
}
