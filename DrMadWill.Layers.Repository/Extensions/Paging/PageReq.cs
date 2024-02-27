namespace DrMadWill.Layers.Repository.Extensions.Paging
{
    /// <summary>
    /// Represents a request for paging.
    /// </summary>
    public class PageReq
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PerPage { get; set; } = 10;
    }
}