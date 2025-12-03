using SupplierGuard.Domain.Interfaces;
using System.Security.Claims;

namespace SupplierGuard.Services
{
    /// <summary>
    /// Implementation of the current user service using HttpContext.
    /// Extracts information from Auth0 JWT.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Gets the UserId from the "sub" (Subject) claim of the JWT token.
        /// Auth0 uses "sub" as the unique user identifier.
        /// </summary>
        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub")
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        /// <summary>
        /// Gets the Email from the "email" claim of the JWT token.
        /// </summary>
        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue("email")
            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        /// <summary>
        /// Checks if there is a currently authenticated user.
        /// </summary>
        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
