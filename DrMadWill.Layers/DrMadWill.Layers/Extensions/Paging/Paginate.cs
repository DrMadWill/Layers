namespace DrMadWill.Layers.Extensions.Paging;

public class Paginate<T>
{
    public static int PerPage { get; set; } = 20;

    public static IQueryable<T> Paging(IQueryable<T> source, int page)
    {
        if (page <= 0) page = 1;
        IQueryable<T> paginatedSource = source.Skip((page - 1) * PerPage).Take(PerPage);
        return paginatedSource;
    }
}