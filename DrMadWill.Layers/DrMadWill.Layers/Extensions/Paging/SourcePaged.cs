using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Extensions.Paging;

public class SourcePaged<T>
{
    public SourcePaged()
    {
    }

    public SourcePaged(bool isIProp)
    {
        PagingModel = new PageModel(0, 0, 0);
        Source = new List<T>();
    }

    public PageModel PagingModel { get; set; }

    public IList<T> Source { get; set; }
    public static async Task<SourcePaged<TEntity>> PagedAsync<TEntity>(IQueryable<TEntity> source, PageReq req)
            where TEntity : class,new()
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync(),
        };
    }


}