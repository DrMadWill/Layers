namespace DrMadWill.Layers.Repository.Extensions.Paging
{
    /// <summary>
    /// Represents a model for pagination.
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PerPage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageModel"/> class.
        /// </summary>
        /// <param name="totalItems">The total number of items.</param>
        /// <param name="currentPage">The current page number.</param>
        /// <param name="perPage">The number of items per page.</param>
        public PageModel(int totalItems, int currentPage, int perPage)
        {
            CurrentPage = currentPage;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)perPage);
            PerPage = perPage;
        }

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPreviousPage => (CurrentPage > 1);

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNextPage => (CurrentPage < TotalPages);
    }
}