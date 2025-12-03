using FluentValidation;
using MediatR;

namespace SupplierGuard.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior for automatic validation using FluentValidation.
    /// It is executed before each Handler to validate the Command/Query.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            
            if (!_validators.Any())
            {
                return await next();
            }

            
            var context = new ValidationContext<TRequest>(request);

            
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            
            if (failures.Any())
            {
                throw new Common.Exceptions.ValidationException(failures);
            }

            
            return await next();
        }
    }
}
