using System.Reflection;
using AutoMapper;
using DrMadWill.Layers.Abstractions.Repository.Repositories.CQRS;
using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using DrMadWill.Layers.Concrete.Repository.Sys;
using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.CQRS;

public abstract class QueryRepositories : IQueryRepositories
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly Dictionary<Type, object> _repositories;
    protected readonly Type _assembly;
    public QueryRepositories(DbContext orgContext, IMapper mapper,Type type)
    {
        _dbContext = orgContext;
        _mapper = mapper;
        _repositories = new Dictionary<Type, object>();
        _assembly = type;
    }

    public virtual IReadRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
        where TEntity : class, IBaseEntity<TPrimary>, new()
    {
        if (_repositories.Keys.Contains(typeof(TEntity)))
            return _repositories[typeof(TEntity)] as IReadRepository<TEntity, TPrimary>;

        var repo = new ReadRepository<TEntity, TPrimary>(_dbContext, _mapper);
        _repositories.Add(typeof(TEntity), repo);
        return repo;
    }

    public virtual TRepository SpecialRepository<TRepository>()
    {
        if (_repositories.Keys.Contains(typeof(TRepository)))
            return (TRepository)_repositories[typeof(TRepository)];

        var type = _assembly.Assembly.GetTypes()
            .FirstOrDefault(x => !x.IsAbstract
                                 && !x.IsInterface
                                 && x.Name == typeof(TRepository).Name.Substring(1));

        if (type == null)
            throw new KeyNotFoundException($"Repository type is not found.{ typeof(TRepository).Name.Substring(1)}");

        var repository = (TRepository)Activator.CreateInstance(type, _dbContext, _mapper)!;

        _repositories.Add(typeof(TRepository), repository);

        return repository;
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
            //_dbContext
            _dbContext?.Dispose();

            // _repositories clear
            foreach (var repository in _repositories.Values)
                if (repository is IDisposable disposableRepo)
                    disposableRepo.Dispose();

            _repositories.Clear();
        }
    }

    ~QueryRepositories()
    {
        Dispose(false);
    }
}