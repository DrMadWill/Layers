using AutoMapper;
using DrMadWill.Layers.Core.Abstractions;
using DrMadWill.Layers.Repository.Abstractions.CQRS;
using DrMadWill.Layers.Repository.Abstractions.Sys;
using DrMadWill.Layers.Repository.Concretes.Sys;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Repository.Concretes.CQRS
{
    /// <summary>
    /// Base class for unit of work implementations.
    /// </summary>
    public abstract class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected readonly DbContext DbContext;
        protected readonly Dictionary<Type, object> Repositories;
        protected readonly Type Assembly;
        protected readonly IMapper Mapper;

        /// <summary>
        /// Constructor for UnitOfWork.
        /// </summary>
        /// <param name="context">The DbContext to be used for unit of work.</param>
        /// <param name="type">The Type used for assembly information.</param>
        /// <param name="mapper">The IMapper for mapping class</param>
        public UnitOfWork(DbContext context, Type type, IMapper mapper)
        {
            DbContext = context;
            Repositories = new Dictionary<Type, object>();
            Assembly = type;
            Mapper = mapper;
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

        public virtual async Task SynchronizationData<TEvent, TEntity, TPrimary>(TEvent @event,Action<string>? log = null) 
            where TEvent : class, IHasDelete 
            where TEntity : class, IOriginEntity<TPrimary>, new()
        {
            var repo = OriginRepository<TEntity, TPrimary>();
            var dict = Mapper.Map<TEntity>(@event);
            if (@event.IsDeleted == true)
            {
                try
                {
                    await repo.RemoveAsync(dict);
                    await CommitAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    log?.Invoke(nameof(TEvent) + $" error occur | not deleted | err : {e}");
                }
            }
            else
            {
                if (await repo.Table.AnyAsync(s => s.Id.Equals(dict.Id))) await repo.UpdateAsync(dict);
                else await repo.AddAsync(dict);
                await CommitAsync();
            }
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
        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
