using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using DrMadWill.Layers.Core;

namespace DrMadWill.Layers.Abstractions.Repository.Repositories.CQRS;

public interface IQueryRepositories : IDisposable
{
    IReadRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
        where TEntity : class, IBaseEntity<TPrimary>, new();

    TRepository SpecialRepository<TRepository>();
}