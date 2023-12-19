using System.Linq.Expressions;
using DrMadWill.Layers.Abstractions.Repository.Core;

namespace DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;

public interface IWriteRepository<TEntity, in TPrimary> : IRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()

{
    Task<TEntity> AddAsync(TEntity entity);

    TEntity Update(TEntity entity);

    TEntity Delete(TEntity entity);

    Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate);
}