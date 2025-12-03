using FluentValidation;
using MediatR;
using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Domain.Interfaces;
using SupplierGuard.Domain.Repositories;

namespace SupplierGuard.Application.Suppliers.Commands.DeleteSupplier
{
    /// <summary>
    /// Command to remove a provider.
    /// </summary>
    public record DeleteSupplierCommand : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }

    /// <summary>
    /// Handler for DeleteSupplierCommand.
    /// </summary>
    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Unit>
    {
        private readonly ISupplierRepository _repository;
        private readonly IApplicationDbContext _context;

        public DeleteSupplierCommandHandler(
            ISupplierRepository repository,
            IApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (supplier == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.Supplier), request.Id);
            }

            _repository.Delete(supplier);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

    /// <summary>
    /// Validator for DeleteSupplierCommand.
    /// </summary>
    public class DeleteSupplierCommandValidator : AbstractValidator<DeleteSupplierCommand>
    {
        public DeleteSupplierCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Supplier ID is required.");
        }
    }

}
