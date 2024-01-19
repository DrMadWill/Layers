using System.Linq.Expressions;
using DrMadWill.Layers.Core;
using DrMadWill.Layers.Extensions.Paging;

namespace DrMadWill.Layers.Abstractions.Repository.Sys;

public interface IReadRepository<TEntity, TPrimary> : IRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    IQueryable<TEntity> GetAllQueryable(bool tracking = false, bool isDeleted = false);

    IQueryable<TEntity> GetAllIncludingQueryable(bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindByQueryable(Expression<Func<TEntity, bool>> predicate);

    IQueryable<TEntity> FindByIncludingQueryable(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<List<TEntity>> GetAllListAsync(bool isDeleted = false);

    Task<List<TDto>> GetAllListAsync<TDto>(bool isDeleted = false)
        where TDto : class, IBaseDto<TPrimary>;

    Task<List<TDto>> GetAllListAsync<TDto>(string languageCode, bool isDeleted = false)
        where TDto : class, IBaseDto<TPrimary>;

    Task<List<TEntity>> GetAllListIncludingAsync(bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<List<TDto>> GetAllListIncludingAsync<TDto>(bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;

    Task<List<TDto>> GetAllListIncludingAsync<TDto>(string languageCode, bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;

    ValueTask<TEntity> FindAsync(TPrimary id);

    ValueTask<TDto> FindAsync<TDto>(TPrimary id)
        where TDto : class, IBaseDto<TPrimary>;

    ValueTask<TDto> FindAsync<TDto>(string languageCode, TPrimary id)
        where TDto : class, IBaseDto<TPrimary>;

    Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

    Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
        where TDto : class, IBaseDto<TPrimary>;

    Task<TDto?> GetFirstAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TEntity>> GetSourcePagedAsync(Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties);

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, bool isDeleted = false);

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, bool isDeleted = false)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req, bool isDeleted = false)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null);

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req,
        Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null);

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req, Func<List<TEntity>, List<TEntity>>? func = null)
        where TDto : class, IBaseDto<TPrimary>;

    Task<SourcePaged<TEntity>> GetSourcePagedAsync(IQueryable<TEntity> source, PageReq req);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

    Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate);

    Task<int> CountAsync();

    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
}