using DrMadWill.Layers.Abstractions.Repository.CQRS;
using DrMadWill.Layers.Abstractions.Repository.Sys;
using DrMadWill.Layers.Concrete.Repository.Sys;
using DrMadWill.Layers.Core;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.CQRS
{
    /// <summary>
    /// Base class for unit of work implementations.
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected readonly DbContext DbContext;
        protected readonly Dictionary<Type, object> Repositories;
        protected readonly Type Assembly;

        /// <summary>
        /// Constructor for UnitOfWork.
        /// </summary>
        /// <param name="orgContext">The DbContext to be used for unit of work.</param>
        /// <param name="type">The Type used for assembly information.</param>
        public UnitOfWork(DbContext orgContext, Type type)
        {
            DbContext = orgContext;
            Repositories = new Dictionary<Type, object>();
            Assembly = type;
        }

        /// <summary>
        /// Gets a write repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TPrimary">The primary key type.</typeparam>
        /// <returns>An instance of the write repository for the specified entity type.</returns>
        public virtual IWriteRepository<TEntity, TPrimary> Repository<TEntity, TPrimary>()
            where TEntity : class, IBaseEntity<TPrimary>, new()
        {
            if (Repositories.Keys.Contains(typeof(TEntity)))
                return Repositories[typeof(TEntity)] as IWriteRepository<TEntity, TPrimary>;

            var repo = new WriteRepository<TEntity, TPrimary>(DbContext);
            Repositories.Add(typeof(TEntity), repo);
            return repo;
        }

        public IWriteOriginRepository<TEntity, TPrimary> OriginRepository<TEntity, TPrimary>() where TEntity : class, IOriginEntity<TPrimary>, new()
        {
            if (Repositories.Keys.Contains(typeof(TEntity)))
                return Repositories[typeof(TEntity)] as IWriteOriginRepository<TEntity, TPrimary>;

            var repo = new WriteOriginRepository<TEntity, TPrimary>(DbContext);
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

            var repository = (TRepository)Activator.CreateInstance(type, DbContext)!;

            Repositories.Add(typeof(TRepository), repository);

            return repository;
        }

        /// <summary>
        /// Asynchronously commits changes to the DbContext.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual Task CommitAsync() => DbContext.SaveChangesAsync();

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
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
