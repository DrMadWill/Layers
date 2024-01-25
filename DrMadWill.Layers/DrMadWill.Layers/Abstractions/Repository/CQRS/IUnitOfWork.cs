using DrMadWill.Layers.Abstractions.Repository.Sys;
using DrMadWill.Layers.Core;

namespace DrMadWill.Layers.Abstractions.Repository.CQRS;
/// <summary>
/// Base class for unit of work implementations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets a write repository for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TPrimary">The primary key type.</typeparam>
    /// <returns>An instance of the write repository for the specified entity type.</returns>
    IWriteRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
        where TEntity : class, IBaseEntity<TPrimary>, new();
    /// <summary>
    /// Gets a special repository based on the provided type.
    /// </summary>
    /// <typeparam name="TRepository">The type of the special repository.</typeparam>
    /// <returns>An instance of the special repository.</returns>
    TRepository SpecialRepository<TRepository>();

    /// <summary>
    /// Asynchronously commits changes to the DbContext.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CommitAsync();
}