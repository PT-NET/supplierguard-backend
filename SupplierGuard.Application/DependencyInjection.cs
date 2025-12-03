using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SupplierGuard.Application.Common.Behaviors;
using System.Reflection;

namespace SupplierGuard.Application
{
    /// <summary>
    /// Extension class for registering Application layer services.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Register all services in the Application layer.
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // ===== MEDIATR (CQRS) =====
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
            });

            // ===== AUTOMAPPER =====
            services.AddAutoMapper(assembly);

            // ===== FLUENTVALIDATION =====
            services.AddValidatorsFromAssembly(assembly);

            // ===== MEDIATR BEHAVIORS (PIPELINE) =====
            // Order matters: they are executed in the order they are registered
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            return services;
        }
    }

}
