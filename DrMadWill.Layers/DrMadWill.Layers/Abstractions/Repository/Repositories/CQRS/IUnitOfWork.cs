using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using DrMadWill.Layers.Core;

namespace DrMadWill.Layers.Abstractions.Repository.Repositories.CQRS;

public interface IUnitOfWork : IDisposable
{
    IWriteRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
        where TEntity : class, IBaseEntity<TPrimary>, new();

    TRepository SpecialRepository<TRepository>();

    public Task CommitAsync();
}