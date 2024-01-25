namespace DrMadWill.Layers.Extensions.Paging
{
    /// <summary>
    /// Represents a request for paging.
    /// </summary>
    public class PageReq
    {
        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PerPage { get; set; }
    }
}