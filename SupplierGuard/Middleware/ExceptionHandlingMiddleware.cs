using SupplierGuard.Application.Common.Exceptions;
using SupplierGuard.Models;
using System.Net;
using System.Text.Json;

namespace SupplierGuard.Middleware
{
    /// <summary>
    /// Middleware for global exception handling.
    /// It catches all exceptions and returns appropriate HTTP responses.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            ApiResponse<object> apiResponse;

            switch (exception)
            {
                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse<object>.Fail(validationException.Errors);
                    break;

                case NotFoundException notFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    apiResponse = ApiResponse<object>.Fail(notFoundException.Message);
                    break;

                case ConflictException conflictException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    apiResponse = ApiResponse<object>.Fail(conflictException.Message);
                    break;

                case BadRequestException badRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    apiResponse = ApiResponse<object>.Fail(badRequestException.Message);
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    apiResponse = ApiResponse<object>.Fail("Unauthorized access.");
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    apiResponse = ApiResponse<object>.Fail(
                        context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                            ? exception.Message
                            : "An internal server error occurred.");
                    break;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var result = JsonSerializer.Serialize(apiResponse, options);
            await response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Extension to register the middleware.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
