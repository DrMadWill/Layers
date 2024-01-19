using System.Linq.Expressions;
using DrMadWill.Layers.Abstractions.Repository.Sys;
using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.Sys;

public class WriteRepository<TEntity, TPrimary> : IWriteRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()

{
    protected readonly DbContext DbContext;
    public DbSet<TEntity> Table { get; private set; }

    public WriteRepository(DbContext dbContext)
    {
        DbContext = dbContext;
        Table = dbContext.Set<TEntity>();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.CreatedDate = DateTime.Now;
        await Table.AddAsync(entity);
        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        entities.ForEach(entitiy => entitiy.CreatedDate = DateTime.Now);
        await Table.AddRangeAsync(entities);
        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedDate = DateTime.Now;
        Table.Update(entity);
        //_dbContext.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities)
    {
        entities.ForEach(entity => entity.UpdatedDate = DateTime.Now);
        Table.UpdateRange(entities);
        //_dbContext.Entry(entity).State = EntityState.Modified;
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        entity.IsDeleted = true;
        Table.Update(entity);
        // _dbContext.Entry(entity).State = EntityState.Deleted;
        return entity;
    }

    public async Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities)
    {
        entities.ForEach((e) => e.IsDeleted = true);
        Table.UpdateRange(entities);
        // _dbContext.Entry(entity).State = EntityState.Deleted;
        return entities;
    }

    public async Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        List<TEntity> entities = Table.Where(predicate).ToList();
        await DeleteRangeAsync(entities);
    }

    public async Task<TEntity> DeleteByIdAsync(TPrimary id)
    {
        var entity = await Table.FirstOrDefaultAsync(x => x.Id.Equals(id));
        if (entity == null) return null;
        return await DeleteAsync(entity);
    }

    public async Task<TEntity> RemoveAsync(TEntity entity) // Hard Delete
    {
        Table.Remove(entity);
        // _dbContext.Entry(entity).State = EntityState.Deleted;
        return entity;
    }

    public async Task<List<TEntity>> RemoveRangeAsync(List<TEntity> entities) // Hard Delete
    {
        Table.RemoveRange(entities);
        // _dbContext.Entry(entity).State = EntityState.Deleted;
        return entities;
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    ~WriteRepository()
    {
        Dispose(false);
    }
}