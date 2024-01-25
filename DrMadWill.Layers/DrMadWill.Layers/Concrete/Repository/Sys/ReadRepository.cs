using System.Linq.Expressions;
using AutoMapper;
using DrMadWill.Layers.Abstractions.Repository.Sys;
using DrMadWill.Layers.Core;
using DrMadWill.Layers.Extensions;
using DrMadWill.Layers.Extensions.Paging;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.Sys;

/// <summary>
/// Represents a generic read-only repository for accessing entities of type TEntity with primary key of type TPrimary.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TPrimary">The type of the primary key for the entity.</typeparam>
public class ReadRepository<TEntity, TPrimary> : IReadRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    protected readonly DbContext _dbContext;
    protected readonly IMapper _mapper;

    /// <summary>
    /// Gets the table of the TEntity type from the database context.
    /// </summary>
    public DbSet<TEntity> Table { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadRepository{TEntity, TPrimary}"/> class.
    /// </summary>
    /// <param name="dbContext">The database context to be used by the repository.</param>
    /// <param name="mapper">The AutoMapper instance for entity-DTO mappings.</param>
    public ReadRepository(DbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
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
    ~ReadRepository()
    {
        Dispose(false);
    }

    /// <summary>
    /// Binds include properties to the given query.
    /// </summary>
    /// <param name="query">The query to which the include properties will be added.</param>
    /// <param name="includeProperties">A collection of expressions indicating the properties to be included in the query.</param>
    private void BindIncludeProperties(IQueryable<TEntity> query, IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
    {
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity, optionally including deleted entities and/or applying tracking.
    /// </summary>
    /// <param name="tracking">If true, the query will track changes to the entities. Default is false.</param>
    /// <param name="isDeleted">If true, the query will include entities that are marked as deleted. Default is false.</param>
    /// <returns>An IQueryable of all entities of type TEntity.</returns>
    public virtual IQueryable<TEntity> GetAllQueryable(bool tracking = false, bool isDeleted = false)
    {
        return BehaviorDeleteStatus(tracking ? Table : Table.AsNoTracking());

        IQueryable<TEntity> BehaviorDeleteStatus(IQueryable<TEntity> entities)
            => isDeleted ? entities : entities.Where(s => s.IsDeleted != true); 
    }

    
    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity, including specified related entities.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>An IQueryable of all entities of type TEntity with specified related entities included.</returns>
    public virtual IQueryable<TEntity> GetAllIncludingQueryable(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = GetAllQueryable();
        BindIncludeProperties(query, includeProperties);
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return query;
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity as a list.
    /// </summary>
    /// <param name="isDeleted">If true, includes entities marked as deleted in the result. Default is false.</param>
    /// <param name="tracking">If true, the query will track changes to the entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity.</returns>
    public virtual Task<List<TEntity>> GetAllListAsync(bool isDeleted = false, bool tracking = false) =>
        GetAllQueryable(tracking, isDeleted).ToListAsync();


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity as a list and maps them to a list of DTOs of type TDto.
    /// </summary>
    /// <param name="isDeleted">If true, includes entities marked as deleted in the result. Default is false.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of DTOs of type TDto mapped from the entities of type TEntity.</returns>
    public virtual async Task<List<TDto>> GetAllListAsync<TDto>(bool isDeleted = false)
        where TDto : class, IBaseDto<TPrimary>
        => (await GetAllListAsync(isDeleted)).Select(_mapper.Map<TDto>).ToList();


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity, maps them to a list of DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="isDeleted">If true, includes entities marked as deleted in the result. Default is false.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a localized list of DTOs of type TDto.</returns>
    public virtual async Task<List<TDto>> GetAllListAsync<TDto>(string languageCode, bool isDeleted = false) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetAllListAsync(isDeleted);
        var mapDto = data.Select(_mapper.Map<TDto>).ToList();
        return LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, mapDto, languageCode).ToList();
    }

    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity including specified related entities and returns them as a list.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity with specified related entities included.</returns>
    public virtual Task<List<TEntity>> GetAllListIncludingAsync(
        params Expression<Func<TEntity, object>>[] includeProperties)
        => GetAllIncludingQueryable(includeProperties).ToListAsync();


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity including specified related entities, and maps them to a list of DTOs of type TDto.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of DTOs of type TDto with specified related entities included.</returns>
    public async Task<List<TDto>> GetAllListIncludingAsync<TDto>(
        params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
        => (await GetAllListIncludingAsync(includeProperties)).Select(_mapper.Map<TDto>).ToList();

    public virtual async Task<List<TDto>> GetAllListIncludingAsync<TDto>(string languageCode, params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetAllListIncludingAsync(includeProperties);
        return LanguageHelper
            .GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
            .ToList();
    }

    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the entity found, or null if not found.</returns>
    public virtual async ValueTask<TEntity> FindAsync(TPrimary id)
        => await Table.FindAsync(id);


    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key and maps it to a DTO of type TDto.
    /// </summary>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the DTO of type TDto mapped from the found entity, or null if not found.</returns>
    public virtual async ValueTask<TDto> FindAsync<TDto>(TPrimary id) where TDto : class, IBaseDto<TPrimary>
        => _mapper.Map<TDto>(await FindAsync(id));


    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key, maps it to a DTO of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the localized DTO of type TDto.</returns>
    public virtual async ValueTask<TDto> FindAsync<TDto>(string languageCode, TPrimary id) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await FindAsync(id);
        return LanguageHelper.GetLocalized(data, _mapper.Map<TDto>(data), languageCode);
    }


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the first entity that satisfies the condition, or null if no such entity is found.</returns>
    public virtual Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate) =>
        GetAllQueryable().FirstOrDefaultAsync(predicate);


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition and maps it to a DTO of type TDto.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains the DTO of type TDto mapped from the first entity that satisfies the condition, or null if no such entity is found.</returns>
    public async Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetFirstAsync(predicate);
        return data == null ? null : _mapper.Map<TDto>(data);
    }


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition, maps it to a DTO of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains the localized DTO of type TDto.</returns>
    public virtual async Task<TDto?> GetFirstAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate)
        where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetFirstAsync(predicate);
        return data == null ? null : _mapper.Map<TDto>(data);
    }


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
    public virtual IQueryable<TEntity> FindByIncludingQueryable(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
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
    /// DTO and IQueryable Source | Asynchronously retrieves a paged result from a given IQueryable source and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(IQueryable<TEntity> source, PageReq req)
        where TDto : class, IBaseDto<TPrimary>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()).Select(_mapper.Map<TDto>).ToList(),
        };
    }


    /// <summary>
    /// Language Support IQueryable Source | Asynchronously retrieves a paged result from a given IQueryable source, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the request details are null.</exception>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(IQueryable<TEntity> source, PageReq req, string languageCode)
        where TDto : class, IBaseDto<TPrimary>
    {
        if (req == null) throw new ArgumentNullException(nameof(req) + " is null");
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        var data = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync();
        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
        };
    }

    
    
    
    /// <summary>
    /// Predicate | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate and includes specified related entities.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities that satisfy the condition.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(Expression<Func<TEntity, bool>> predicate, PageReq req, params Expression<Func<TEntity, object>>[] includeProperties)
        => await GetSourcePagedAsync(GetAllIncludingQueryable(includeProperties).Where(predicate), req);


    /// <summary>
    /// DTO and Predicate Overload | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate, includes specified related entities, and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto that satisfy the condition.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req, params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>
        => await GetSourcePagedAsync<TDto>(GetAllIncludingQueryable(includeProperties).Where(predicate), req);
    
    /// <summary>
    ///  Language and Predicate Support | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate, includes specified related entities, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto that satisfy the condition.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req, string languageCode, params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>
        => await GetSourcePagedAsync<TDto>(GetAllIncludingQueryable(includeProperties).Where(predicate), req, languageCode);

    
    
        
    /// <summary>
    /// Query Search | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom query function.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        => await GetSourcePagedAsync(func == null ? GetAllQueryable() : func(GetAllQueryable()), req);
    
    /// <summary>
    /// DTO and Query Search | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto after applying the custom query function.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>
        => await GetSourcePagedAsync<TDto>(func == null ? GetAllQueryable() : func(GetAllQueryable()), req);
    
    /// <summary>
    /// Language, DTO, and Query Search Support | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto after applying the custom query function.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, string languageCode, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>
        => await GetSourcePagedAsync<TDto>(func == null ? GetAllQueryable() : func(GetAllQueryable()), req, languageCode);

    
    
    
  
    /// <summary>
    /// Func | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom function.</returns>
    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null)
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = func == null ? await Paginate<TEntity>.Paging(source, req.Page).ToListAsync() : func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())),
        };
    }
    
    /// <summary>
    /// Func DTO | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto after applying the custom function.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = func == null ? (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()).Select(_mapper.Map<TDto>).ToList() :
                func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())).Select(_mapper.Map<TDto>).ToList(),
        };
    }

    /// <summary>
    /// Func Lang DTO | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto after applying the custom function.</returns>
    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, string languageCode, Func<List<TEntity>, List<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;

        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        var data = func == null
            ? (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())
            : func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()));

        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
        };
    }

}