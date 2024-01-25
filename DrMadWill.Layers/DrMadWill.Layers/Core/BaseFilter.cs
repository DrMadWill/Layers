namespace DrMadWill.Layers.Core;

/// <summary>
/// Base class for entity filtering criteria.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TPrimary">The primary key type.</typeparam>
public abstract class BaseFilter<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    /// <summary>
    /// Gets or sets the filter criteria for the entity's primary key.
    /// </summary>
    public TPrimary? Id { get; set; }

    /// <summary>
    /// Gets or sets the filter criteria for the entity's name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Applies the filter criteria to the source IQueryable.
    /// </summary>
    /// <param name="source">The IQueryable source to be filtered.</param>
    /// <returns>The filtered IQueryable result.</returns>
    public abstract IQueryable<TEntity> Filtered(IQueryable<TEntity> source);
}
