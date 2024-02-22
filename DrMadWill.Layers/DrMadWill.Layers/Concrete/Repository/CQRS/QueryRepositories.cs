using AutoMapper;
using DrMadWill.Layers.Abstractions.Repository.CQRS;
using DrMadWill.Layers.Abstractions.Repository.Sys;
using DrMadWill.Layers.Concrete.Repository.Sys;
using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.CQRS
{
    /// <summary>
    /// Base class for query repositories.
    /// </summary>
    public abstract class QueryRepositories : IQueryRepositories, IDisposable
    {
        protected readonly DbContext DbContext;
        protected readonly IMapper Mapper;
        protected readonly Dictionary<Type, object> Repositories;
        protected readonly Type Assembly;

        /// <summary>
        /// Constructor for QueryRepositories.
        /// </summary>
        /// <param name="orgContext">The DbContext to be used for queries.</param>
        /// <param name="mapper">The AutoMapper instance for mapping entities.</param>
        /// <param name="type">The Type used for assembly information.</param>
        public QueryRepositories(DbContext orgContext, IMapper mapper, Type type)
        {
            DbContext = orgContext;
            Mapper = mapper;
            Repositories = new Dictionary<Type, object>();
            Assembly = type;
        }

        /// <summary>
        /// Gets a repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TPrimary">The primary key type.</typeparam>
        /// <returns>An instance of the read repository for the specified entity type.</returns>
        public virtual IReadRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
            where TEntity : class, IBaseEntity<TPrimary>, new()
        {
            if (Repositories.Keys.Contains(typeof(TEntity)))
                return Repositories[typeof(TEntity)] as IReadRepository<TEntity, TPrimary>;

            var repo = new ReadRepository<TEntity, TPrimary>(DbContext, Mapper);
            Repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public IReadOriginRepository<TEntity, TPrimary> OriginRepository<TEntity, TPrimary>() where TEntity : class, IOriginEntity<TPrimary>, new()
        {
            if (Repositories.Keys.Contains(typeof(TEntity)))
                return Repositories[typeof(TEntity)] as IReadOriginRepository<TEntity, TPrimary>;

            var repo = new ReadOriginRepository<TEntity, TPrimary>(DbContext, Mapper);
            Repositories.Add(typeof(TEntity), repo);
            return repo; 
        }

        /// <summary>
        /// Gets a special repository based on the provided type.
        /// </summary>
        /// <typeparam name="TRepository">The type of the special repository.</typeparam>
        /// <returns>An instance of the special repository.</returns>
        public virtual TRepository SpecialRepository<TRepository>()
        {
            if (Repositories.Keys.Contains(typeof(TRepository)))
                return (TRepository)Repositories[typeof(TRepository)];

            var type = Assembly.Assembly.GetTypes()
                .FirstOrDefault(x => !x.IsAbstract
                                     && !x.IsInterface
                                     && x.Name == typeof(TRepository).Name.Substring(1));

            if (type == null)
                throw new KeyNotFoundException($"Repository type is not found: {typeof(TRepository).Name.Substring(1)}");

            var repository = (TRepository)Activator.CreateInstance(type, DbContext, Mapper)!;

            Repositories.Add(typeof(TRepository), repository);

            return repository;
        }

        /// <summary>
        /// Disposes of the DbContext and clears repository references.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources.
        /// </summary>
        /// <param name="disposing">True if disposing; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of DbContext
                DbContext?.Dispose();

                // Dispose of repositories that implement IDisposable
                foreach (var repository in Repositories.Values)
                {
                    if (repository is IDisposable disposableRepo)
                    {
                        disposableRepo.Dispose();
                    }
                }

                Repositories.Clear();
            }
        }

        /// <summary>
        /// Finalizer to ensure Dispose is called.
        /// </summary>
        ~QueryRepositories()
        {
            Dispose(false);
        }
    }
}
