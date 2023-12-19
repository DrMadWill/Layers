using System.Linq.Expressions;
using DrMadWill.Layers.Abstractions.Repository.Core;
using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.Sys;

public class WriteRepository<TEntity, TPrimary> : IWriteRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()

{
    private readonly DbContext _dbContext;
    public DbSet<TEntity> Table { get; private set; }

    public WriteRepository(DbContext orgContext)
    {
        _dbContext = orgContext;
        Table = orgContext.Set<TEntity>();
    }

    public virtual TEntity Update(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return entity;
    }

    public virtual TEntity Delete(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Deleted;
        return entity;
    }

    public virtual async Task DeleteWhereAsync(Expression<Func<TEntity, bool>> predicate)
    {
        IEnumerable<TEntity> entities = await Table.Where(predicate).ToListAsync();
        foreach (var entity in entities)
        {
            _dbContext.Entry(entity).State = EntityState.Deleted;
        }
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await Table.AddAsync(entity);
        return entity;
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