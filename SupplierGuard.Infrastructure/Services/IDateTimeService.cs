namespace SupplierGuard.Infrastructure.Services
{
    /// <summary>
    /// Interface for date/time system abstraction.
    /// Useful for unit testing and time control.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Gets the current date and time in UTC.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current local date and time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets only the current date (without time).
        /// </summary>
        DateTime Today { get; }
    }

    /// <summary>
    /// Implementation of the date/time service using the operating system.
    /// </summary>
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now => DateTime.Now;

        public DateTime Today => DateTime.Today;
    }
}
