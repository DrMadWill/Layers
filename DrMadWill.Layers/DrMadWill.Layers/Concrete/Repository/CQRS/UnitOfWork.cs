using System.Reflection;
using DrMadWill.Layers.Abstractions.Repository.Repositories.CQRS;
using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using DrMadWill.Layers.Concrete.Repository.Sys;
using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.CQRS;

public abstract class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(DbContext orgContext)
    {
        _dbContext = orgContext;
        _repositories = new Dictionary<Type, object>();
    }

    public virtual IWriteRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
        where TEntity : class, IBaseEntity<TPrimary>, new()
    {
        if (_repositories.Keys.Contains(typeof(TEntity)))
            return _repositories[typeof(TEntity)] as IWriteRepository<TEntity, TPrimary>;

        var repo = new WriteRepository<TEntity, TPrimary>(_dbContext);
        _repositories.Add(typeof(TEntity), repo);
        return repo;
    }

    public virtual TRepository SpecialRepository<TRepository>()
    {
        if (_repositories.Keys.Contains(typeof(TRepository)))
            return (TRepository)_repositories[typeof(TRepository)];

        var type = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(x => !x.IsAbstract
                                 && !x.IsInterface
                                 && x.BaseType == typeof(WriteRepository<,>)
                                 && x.Name == typeof(TRepository).Name.Substring(1));

        if (type == null)
            throw new KeyNotFoundException("Repository type is not found");

        var repository = (TRepository)Activator.CreateInstance(type, _dbContext)!;

        _repositories.Add(typeof(TRepository), repository);

        return repository;
    }

    public virtual Task CommitAsync() => _dbContext.SaveChangesAsync();

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            //_dbContext clear
            _dbContext?.Dispose();

            // _repositories clear
            foreach (var repository in _repositories.Values)
                if (repository is IDisposable disposableRepo)
                    disposableRepo.Dispose();

            _repositories.Clear();
        }
    }

    ~UnitOfWork()
    {
        Dispose(false);
    }
}