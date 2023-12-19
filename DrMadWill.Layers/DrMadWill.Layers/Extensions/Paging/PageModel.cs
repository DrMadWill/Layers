namespace DrMadWill.Layers.Extensions.Paging;

public class PageModel
{
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int PerPage { get; set; }

    public PageModel(int totalItems, int currentPage, int perPage)
    {
        CurrentPage = currentPage;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)perPage);
        PerPage = perPage;
    }

    public bool HasPreviousPage => (CurrentPage > 1);
    public bool HasNextPage => (CurrentPage < TotalPages);
}