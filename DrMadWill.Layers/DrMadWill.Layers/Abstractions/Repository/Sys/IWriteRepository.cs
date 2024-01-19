using System.Linq.Expressions;
using DrMadWill.Layers.Core;

namespace DrMadWill.Layers.Abstractions.Repository.Sys;

public interface IWriteRepository<TEntity, in TPrimary> : IRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()

{
    Task<TEntity> AddAsync(TEntity entity);

    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity);

    Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities);

    Task<TEntity> DeleteAsync(TEntity entity); // Soft Delete

    Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities); // Soft Delete

    Task<TEntity> DeleteByIdAsync(TPrimary id);// Soft Delete

    Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate); //Soft Delete

    
    /// <summary>
    /// Hard Delete
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<TEntity> RemoveAsync(TEntity entity); 

    /// <summary>
    /// Hard Delete
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<List<TEntity>> RemoveRangeAsync(List<TEntity> entities);  
}