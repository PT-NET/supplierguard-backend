namespace SupplierGuard.Application.Common.Models
{
    /// <summary>
    /// It represents a paginated list of items.
    /// </summary>
    public class PaginatedList<T>
    {
        /// <summary>
        /// List of items on the current page.
        /// </summary>
        public List<T> Items { get; }

        /// <summary>
        /// Current page number (starting at 1).
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Page size.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total number of items across all pages.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Total pages.
        /// </summary>
        public int TotalPages { get; }

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        /// <summary>
        /// Create an empty paginated list.
        /// </summary>
        public static PaginatedList<T> Empty()
        {
            return new PaginatedList<T>(new List<T>(), 0, 1, 10);
        }

        /// <summary>
        /// Creates a paginated list from a tuple (items, totalCount).
        /// </summary>
        public static PaginatedList<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            return new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
