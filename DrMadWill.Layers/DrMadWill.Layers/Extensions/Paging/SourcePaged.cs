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
}