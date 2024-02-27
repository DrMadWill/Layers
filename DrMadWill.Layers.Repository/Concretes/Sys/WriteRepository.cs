using System.Linq.Expressions;
using DrMadWill.Layers.Core.Abstractions;
using DrMadWill.Layers.Repository.Abstractions.Sys;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Repository.Concretes.Sys;

/// <summary>
/// Represents a generic repository for write operations on entities of type TEntity with primary key of type TPrimary.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TPrimary">The type of the primary key for the entity.</typeparam>
public class WriteRepository<TEntity, TPrimary> : IWriteRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    protected readonly DbContext DbContext;
    public DbSet<TEntity> Table { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WriteRepository{TEntity, TPrimary}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    public WriteRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        Table = dbContext.Set<TEntity>();
    }

    /// <summary>
    /// Asynchronously adds a new entity to the DbSet.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.Now;
        await Table.AddAsync(entity);
        return entity;
    }


    /// <summary>
    /// Asynchronously adds a range of entities to the DbSet.
    /// </summary>
    /// <param name="entities">The list of entities to add.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of added entities.</returns>
    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        entities.ForEach(entity => entity.CreatedDate = DateTime.Now);
        await Table.AddRangeAsync(entities);
        return entities;
    }


    /// <summary>
    /// Asynchronously updates an existing entity in the DbSet.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedDate = DateTime.Now;
        Table.Update(entity);
        return entity;
    }


    /// <summary>
    /// Asynchronously updates a range of entities in the DbSet.
    /// </summary>
    /// <param name="entities">The list of entities to update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of updated entities.</returns>
    public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities)
    {
        entities.ForEach(entity => entity.UpdatedDate = DateTime.Now);
        Table.UpdateRange(entities);
        return entities;
    }


    /// <summary>
    /// Asynchronously marks an entity as deleted in the DbSet. This is a soft delete operation.
    /// </summary>
    /// <param name="entity">The entity to mark as deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted entity.</returns>
    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        entity.IsDeleted = true;
        Table.Update(entity);
        return entity;
    }


    /// <summary>
    /// Asynchronously marks a range of entities as deleted in the DbSet. This is a soft delete operation.
    /// </summary>
    /// <param name="entities">The list of entities to mark as deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of deleted entities.</returns>
    public async Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities)
    {
        entities.ForEach(e => e.IsDeleted = true);
        Table.UpdateRange(entities);
        return entities;
    }


    /// <summary>
    /// Asynchronously marks entities as deleted based on a predicate. This is a soft delete operation.
    /// </summary>
    /// <param name="predicate">An expression to filter the entities to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        List<TEntity> entities = Table.Where(predicate).ToList();
        await DeleteRangeAsync(entities);
    }


    /// <summary>
    /// Asynchronously marks an entity as deleted based on its identifier. This is a soft delete operation.
    /// </summary>
    /// <param name="id">The identifier of the entity to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted entity, or null if the entity is not found.</returns>
    public async Task<TEntity> DeleteByIdAsync(TPrimary id)
    {
        var entity = await Table.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (entity == null) return null;
        return await DeleteAsync(entity);
    }

    /// <summary>
    /// Hard Delete | Asynchronously removes an entity from the DbSet. This is a hard delete operation.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the removed entity.</returns>
    public async Task<TEntity> RemoveAsync(TEntity entity) // Hard Delete
    {
        Table.Remove(entity);
        return entity;
    }


    /// <summary>
    /// Hard Delete | Asynchronously removes a range of entities from the DbSet. This is a hard delete operation.
    /// </summary>
    /// <param name="entities">The list of entities to remove.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of removed entities.</returns>
    public async Task<List<TEntity>> RemoveRangeAsync(List<TEntity> entities) // Hard Delete
    {
        Table.RemoveRange(entities);
        return entities;
    }


    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected implementation of Dispose pattern.
    /// </summary>
    /// <param name="disposing">Indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Free any other managed objects here.
        }
    }

    /// <summary>
    /// Destructor for WriteRepository.
    /// </summary>
    ~WriteRepository()
    {
        Dispose(false);
    }

}