using System.Linq.Expressions;
using AutoMapper;
using DrMadWill.Layers.Repository.Abstractions.Sys;
using DrMadWill.Layers.Repository.Extensions.Paging;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Repository.Concretes.Sys;

public class AnonymousRepository<TEntity> : IAnonymousRepository<TEntity> 
    where TEntity : class,new()
{
    protected readonly DbContext DbContext;
    protected readonly IMapper Mapper;

    /// <summary>
    /// Gets the table of the TEntity type from the database context.
    /// </summary>
    public DbSet<TEntity> Table { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOriginRepository{TEntity, TPrimary}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    /// <param name="mapper">The AutoMapper instance for entity-DTO mappings.</param>
    public AnonymousRepository(DbContext dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
        Table = dbContext.Set<TEntity>();
    }
    
     /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
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
    /// Destructor for ReadRepository.
    /// </summary>
    ~AnonymousRepository()
    {
        Dispose(false);
    }

    /// <summary>
    /// Binds include properties to the given query.
    /// </summary>
    /// <param name="query">The query to which the include properties will be added.</param>
    /// <param name="includeProperties">A collection of expressions indicating the properties to be included in the query.</param>
    private void BindIncludeProperties(IQueryable<TEntity> query,
        IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
    {
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity, optionally including deleted entities and/or applying tracking.
    /// </summary>
    /// <returns>An IQueryable of all entities of type TEntity.</returns>
    public virtual IQueryable<TEntity> GetAllQueryable()
    {
        return ( Table.AsNoTracking());
    }


    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity, including specified related entities.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>An IQueryable of all entities of type TEntity with specified related entities included.</returns>
    public virtual IQueryable<TEntity> GetAllIncludingQueryable(
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = GetAllQueryable();
        BindIncludeProperties(query, includeProperties);
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return query;
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity as a list.
    /// </summary>
    /// <param name="tracking">If true, the query will track changes to the entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity.</returns>
    public virtual Task<List<TEntity>> GetAllListAsync() =>
        GetAllQueryable().ToListAsync();



    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity including specified related entities and returns them as a list.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity with specified related entities included.</returns>
    public virtual Task<List<TEntity>> GetAllListIncludingAsync(
        params Expression<Func<TEntity, object>>[] includeProperties)
        => GetAllIncludingQueryable(includeProperties).ToListAsync();


    

    

    

    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the first entity that satisfies the condition, or null if no such entity is found.</returns>
    public virtual Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate) =>
        GetAllQueryable().FirstOrDefaultAsync(predicate);

    /// <summary>
    /// Retrieves an IQueryable for entities of type TEntity that satisfy a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>An IQueryable of entities that satisfy the specified condition.</returns>
    public virtual IQueryable<TEntity> FindByQueryable(Expression<Func<TEntity, bool>> predicate) =>
        GetAllQueryable().Where(predicate);


    /// <summary>
    /// Retrieves an IQueryable for entities of type TEntity that satisfy a specified condition, including specified related entities.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>An IQueryable of entities that satisfy the specified condition with specified related entities included.</returns>
    public virtual IQueryable<TEntity> FindByIncludingQueryable(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = GetAllQueryable();
        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return query.Where(predicate);
    }

    /// <summary>
    /// Asynchronously determines whether any entity of type TEntity satisfies a condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result is true if any entities satisfy the condition; otherwise, false.</returns>
    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().AnyAsync(predicate);


    /// <summary>
    /// Asynchronously determines whether all entities of type TEntity satisfy a condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result is true if all entities satisfy the condition; otherwise, false.</returns>
    public virtual Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().AllAsync(predicate);

    /// <summary>
    /// Asynchronously counts the number of entities of type TEntity.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation. The task result contains the number of entities.</returns>
    public virtual Task<int> CountAsync()
        => GetAllQueryable().CountAsync();

    /// <summary>
    /// Asynchronously counts the number of entities of type TEntity satisfying a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the number of entities that satisfy the condition.</returns>
    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().CountAsync(predicate);




    /// <summary>
    /// IQueryable | Asynchronously retrieves a paged result from a given IQueryable source.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(IQueryable<TEntity> source, PageReq req)
        => await SourcePaged<TEntity>.PagedAsync(source, req);

    /// <summary>
    /// Predicate | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate and includes specified related entities.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities that satisfy the condition.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(Expression<Func<TEntity, bool>> predicate,
        PageReq req, params Expression<Func<TEntity, object>>[] includeProperties)
        => await GetSourcePagedAsync(GetAllIncludingQueryable(includeProperties).Where(predicate), req);


    

    /// <summary>
    /// Query Search | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom query function.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        => await GetSourcePagedAsync(func == null ? GetAllQueryable() : func(GetAllQueryable()), req);

  
    
    /// <summary>
    /// Func | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom function.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req,
        Func<List<TEntity>, List<TEntity>>? func = null)
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = func == null
                ? await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()
                : func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())),
        };
    }


    
    
}