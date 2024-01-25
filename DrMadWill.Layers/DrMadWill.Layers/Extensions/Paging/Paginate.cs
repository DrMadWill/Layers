using System.Linq;

namespace DrMadWill.Layers.Extensions.Paging
{
    /// <summary>
    /// Helper class for pagination of IQueryable data.
    /// </summary>
    /// <typeparam name="T">The type of elements in the IQueryable data source.</typeparam>
    public class Paginate<T>
    {
        /// <summary>
        /// Gets or sets the default number of items per page.
        /// </summary>
        public static int PerPage { get; set; } = 20;

        /// <summary>
        /// Paginates an IQueryable data source to retrieve a specific page of results.
        /// </summary>
        /// <param name="source">The IQueryable data source to be paginated.</param>
        /// <param name="page">The page number to retrieve (1-based).</param>
        /// <returns>A paginated IQueryable data source containing the requested page of results.</returns>
        public static IQueryable<T> Paging(IQueryable<T> source, int page)
        {
            if (page <= 0) page = 1;
            IQueryable<T> paginatedSource = source.Skip((page - 1) * PerPage).Take(PerPage);
            return paginatedSource;
        }
    }
}