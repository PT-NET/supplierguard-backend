using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SupplierGuard.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline behavior to detect and log requests that take too long.
    /// Useful for identifying performance bottlenecks.
    /// </summary>
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly Stopwatch _timer;

        
        private const int SlowRequestThresholdMs = 500;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
            _timer = new Stopwatch();
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > SlowRequestThresholdMs)
            {
                var requestName = typeof(TRequest).Name;

                _logger.LogWarning(
                    "Long Running Request: {RequestName} ({ElapsedMilliseconds} milliseconds)",
                    requestName,
                    elapsedMilliseconds);
            }

            return response;
        }
    }
}
