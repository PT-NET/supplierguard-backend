namespace SupplierGuard.Domain.Interfaces
{
    /// <summary>
    /// Service to obtain information about the current authenticated user.
    /// Define the contract to access the user context from any layer.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the ID of the authenticated user (sub claim of Auth0).
        /// Returns null if there is no authenticated user.
        /// </summary>
        string? UserId { get; }

        /// <summary>
        /// It obtains the email of the authenticated user.
        /// Returns null if there is no authenticated user or no email address.
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Indicates if there is currently an authenticated user.
        /// </summary>
        bool IsAuthenticated { get; }
    }
}
