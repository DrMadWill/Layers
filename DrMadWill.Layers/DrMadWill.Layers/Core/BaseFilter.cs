namespace DrMadWill.Layers.Core;

public abstract class BaseFilter<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    public TPrimary? Id { get; set; }
    public string? Name { get; set; }

    public abstract IQueryable<TEntity> Filtered(IQueryable<TEntity> source);
}
