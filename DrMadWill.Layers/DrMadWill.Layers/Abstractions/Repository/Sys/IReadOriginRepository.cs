using System.Linq.Expressions;
using DrMadWill.Layers.Core;
using DrMadWill.Layers.Extensions.Paging;

namespace DrMadWill.Layers.Abstractions.Repository.Sys;

public interface IReadOriginRepository<TEntity, TPrimary>: IRepository<TEntity, TPrimary>
    where TEntity : class, IOriginEntity <TPrimary>,new()
{
    
    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity applying tracking.
    /// </summary>
    /// <param name="tracking">If true, the query will track changes to the entities. Default is false.</param>
    /// <returns>An IQueryable of all entities of type TEntity.</returns>
    IQueryable<TEntity> GetAllQueryable(bool tracking = false);


    /// <summary>
    /// Retrieves an IQueryable for all entities of type TEntity, including specified related entities.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>An IQueryable of all entities of type TEntity with specified related entities included.</returns>
    IQueryable<TEntity> GetAllIncludingQueryable(params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity as a list.
    /// </summary>
    /// <param name="tracking">If true, the query will track changes to the entities. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity.</returns>
    Task<List<TEntity>> GetAllListAsync( bool tracking = false);


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity as a list and maps them to a list of DTOs of type TDto.
    /// </summary>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of DTOs of type TDto mapped from the entities of type TEntity.</returns>
    Task<List<TDto>> GetAllListAsync<TDto>()
        where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity, maps them to a list of DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a localized list of DTOs of type TDto.</returns>
    Task<List<TDto>> GetAllListAsync<TDto>(string languageCode)
        where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity including specified related entities and returns them as a list.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all entities of type TEntity with specified related entities included.</returns>
    Task<List<TEntity>> GetAllListIncludingAsync(
        params Expression<Func<TEntity, object>>[] includeProperties);


    /// <summary>
    /// Asynchronously retrieves all entities of type TEntity including specified related entities, and maps them to a list of DTOs of type TDto.
    /// </summary>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of DTOs of type TDto with specified related entities included.</returns>
    Task<List<TDto>> GetAllListIncludingAsync<TDto>(
        params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>;

    Task<List<TDto>> GetAllListIncludingAsync<TDto>(string languageCode,
        params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key.
    /// </summary>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the entity found, or null if not found.</returns>
    ValueTask<TEntity> FindAsync(TPrimary id);


    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key and maps it to a DTO of type TDto.
    /// </summary>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the DTO of type TDto mapped from the found entity, or null if not found.</returns>
    ValueTask<TDto> FindAsync<TDto>(TPrimary id) where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Asynchronously finds an entity of type TEntity by its primary key, maps it to a DTO of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="id">The primary key of the entity to find.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A ValueTask representing the asynchronous operation. The task result contains the localized DTO of type TDto.</returns>
    ValueTask<TDto> FindAsync<TDto>(string languageCode, TPrimary id) where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the first entity that satisfies the condition, or null if no such entity is found.</returns>
    Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition and maps it to a DTO of type TDto.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains the DTO of type TDto mapped from the first entity that satisfies the condition, or null if no such entity is found.</returns>
    Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate) where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Asynchronously retrieves the first entity of type TEntity that satisfies a specified condition, maps it to a DTO of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entity will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains the localized DTO of type TDto.</returns>
    Task<TDto?> GetFirstAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate)
        where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Retrieves an IQueryable for entities of type TEntity that satisfy a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>An IQueryable of entities that satisfy the specified condition.</returns>
    IQueryable<TEntity> FindByQueryable(Expression<Func<TEntity, bool>> predicate);


    /// <summary>
    /// Retrieves an IQueryable for entities of type TEntity that satisfy a specified condition, including specified related entities.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>An IQueryable of entities that satisfy the specified condition with specified related entities included.</returns>
    IQueryable<TEntity> FindByIncludingQueryable(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties);

    /// <summary>
    /// Asynchronously determines whether any entity of type TEntity satisfies a condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result is true if any entities satisfy the condition; otherwise, false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);


    /// <summary>
    /// Asynchronously determines whether all entities of type TEntity satisfy a condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result is true if all entities satisfy the condition; otherwise, false.</returns>
    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Asynchronously counts the number of entities of type TEntity.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation. The task result contains the number of entities.</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Asynchronously counts the number of entities of type TEntity satisfying a specified condition.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains the number of entities that satisfy the condition.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);


    /// <summary>
    /// IQueryable | Asynchronously retrieves a paged result from a given IQueryable source.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities.</returns>
    Task<SourcePaged<TEntity>> GetSourcePagedAsync(IQueryable<TEntity> source, PageReq req);


    /// <summary>
    /// DTO and IQueryable Source | Asynchronously retrieves a paged result from a given IQueryable source and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source is null.</exception>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(IQueryable<TEntity> source, PageReq req)
        where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Language Support IQueryable Source | Asynchronously retrieves a paged result from a given IQueryable source, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="source">The IQueryable source to paginate.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the request details are null.</exception>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(IQueryable<TEntity> source, PageReq req, string languageCode)
        where TDto : class, IBaseDto<TPrimary>;


    /// <summary>
    /// Predicate | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate and includes specified related entities.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities that satisfy the condition.</returns>
    Task<SourcePaged<TEntity>> GetSourcePagedAsync(Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties);


    /// <summary>
    /// DTO and Predicate Overload | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate, includes specified related entities, and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto that satisfy the condition.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    ///  Language and Predicate Support | Asynchronously retrieves a paged result for entities of type TEntity that satisfy a given predicate, includes specified related entities, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="predicate">An expression to test each entity for a condition.</param>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="includeProperties">Expressions indicating the related entities to include in the query.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto that satisfy the condition.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req,
        string languageCode, params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;



    /// <summary>
    /// Query Search | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom query function.</returns>
    Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null);

    /// <summary>
    /// DTO and Query Search | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto after applying the custom query function.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    /// Language, DTO, and Query Search Support | Asynchronously retrieves a paged result using a custom query function applied to the entities of type TEntity, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="func">A function to transform the IQueryable before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto after applying the custom query function.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, string languageCode,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    /// Func | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of entities after applying the custom function.</returns>
    Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null);

    /// <summary>
    /// Func DTO | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity and maps the results to DTOs of type TDto.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of DTOs of type TDto after applying the custom function.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    /// <summary>
    /// Func Lang DTO | Asynchronously retrieves a paged result using a custom function applied to a list of entities of type TEntity, maps the results to DTOs of type TDto, and applies localization based on the specified language code.
    /// </summary>
    /// <param name="req">The pagination request details.</param>
    /// <param name="languageCode">The language code to apply for localization.</param>
    /// <param name="func">A function to transform the list of entities before pagination is applied.</param>
    /// <typeparam name="TDto">The type of data transfer object to which the entities will be mapped.</typeparam>
    /// <returns>A Task representing the asynchronous operation. The task result contains a paged result of localized DTOs of type TDto after applying the custom function.</returns>
    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, string languageCode,
        Func<List<TEntity>, List<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>;
}