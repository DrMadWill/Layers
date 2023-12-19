using System.Linq.Expressions;
using AutoMapper;
using DrMadWill.Layers.Abstractions.Repository.Core;
using DrMadWill.Layers.Abstractions.Repository.Repositories.Sys;
using DrMadWill.Layers.Extensions;
using DrMadWill.Layers.Extensions.Paging;
using Microsoft.EntityFrameworkCore;

namespace DrMadWill.Layers.Concrete.Repository.Sys;

public class ReadRepository<TEntity, TPrimary> : IReadRepository<TEntity, TPrimary>
    where TEntity : class, IBaseEntity<TPrimary>, new()
{
    private readonly DbContext _dbContext;
    private readonly IMapper _mapper;
    public DbSet<TEntity> Table { get; private set; }

    public ReadRepository(DbContext orgContext, IMapper mapper)
    {
        _dbContext = orgContext;
        _mapper = mapper;
        Table = orgContext.Set<TEntity>();
    }

    public void Dispose()
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

    ~ReadRepository()
    {
        Dispose(false);
    }

    private void BindIncludeProperties(IQueryable<TEntity> query, IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
    {
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    public virtual IQueryable<TEntity> GetAllQueryable(bool isDeleted = false) =>
        isDeleted ? Table : Table.Where(s => s.IsDeleted != true);

    public virtual IQueryable<TEntity> GetAllIncludingQueryable(bool isDeleted = false,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = GetAllQueryable(isDeleted);
        BindIncludeProperties(query, includeProperties);
        includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return query;
    }

    public virtual Task<List<TEntity>> GetAllListAsync(bool isDeleted = false) =>
        GetAllQueryable(isDeleted).ToListAsync();

    public virtual async Task<List<TDto>> GetAllListAsync<TDto>(bool isDeleted = false) where TDto : class, IBaseDto<TPrimary>
    {
        return (await GetAllListAsync(isDeleted)).Select(_mapper.Map<TDto>).ToList();
    }

    public virtual async Task<List<TDto>> GetAllListAsync<TDto>(string languageCode, bool isDeleted = false) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetAllListAsync(isDeleted);
        var mapDto = data.Select(_mapper.Map<TDto>).ToList();
        return LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, mapDto, languageCode).ToList();
    }

    public virtual Task<List<TEntity>> GetAllListIncludingAsync(bool isDeleted = false,
        params Expression<Func<TEntity, object>>[] includeProperties)
        => GetAllIncludingQueryable(isDeleted, includeProperties).ToListAsync();

    public async Task<List<TDto>> GetAllListIncludingAsync<TDto>(bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
    {
        return (await GetAllListIncludingAsync(isDeleted, includeProperties)).Select(_mapper.Map<TDto>).ToList();
    }

    public virtual async Task<List<TDto>> GetAllListIncludingAsync<TDto>(string languageCode, bool isDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetAllListIncludingAsync(isDeleted, includeProperties);

        return LanguageHelper
            .GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
            .ToList();
    }

    public virtual async ValueTask<TEntity> FindAsync(TPrimary id)
    {
        return await Table.FindAsync(id);
    }

    public virtual async ValueTask<TDto> FindAsync<TDto>(TPrimary id) where TDto : class, IBaseDto<TPrimary>
    {
        return _mapper.Map<TDto>(await FindAsync(id));
    }

    public virtual async ValueTask<TDto> FindAsync<TDto>(string languageCode, TPrimary id) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await FindAsync(id);
        return LanguageHelper.GetLocalized(data, _mapper.Map<TDto>(data), languageCode);
    }

    public virtual Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate) =>
        GetAllQueryable().FirstOrDefaultAsync(predicate);

    public async Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate) where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetFirstAsync(predicate);
        return data == null ? null : _mapper.Map<TDto>(data);
    }

    public virtual async Task<TDto?> GetFirstAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate)
        where TDto : class, IBaseDto<TPrimary>
    {
        var data = await GetFirstAsync(predicate);
        return data == null ? null : _mapper.Map<TDto>(data);
    }

    public virtual IQueryable<TEntity> FindByQueryable(Expression<Func<TEntity, bool>> predicate) =>
        GetAllQueryable().Where(predicate);

    public virtual IQueryable<TEntity> FindByIncludingQueryable(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = GetAllQueryable();
        query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        return query.Where(predicate);
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;
        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;
        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = func == null ? (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()).Select(_mapper.Map<TDto>).ToList() :
                func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())).Select(_mapper.Map<TDto>).ToList(),
        };
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req, Func<List<TEntity>, List<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;
        // PerPage Count
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

    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(Expression<Func<TEntity, bool>> predicate, PageReq req, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var source = GetAllIncludingQueryable(false, includeProperties);
        return await GetSourcePagedAsync(source, req);
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(Expression<Func<TEntity, bool>> predicate, PageReq req, params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllIncludingQueryable(false, includeProperties);
        return await GetSourcePagedAsync<TDto>(source, req);
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, Expression<Func<TEntity, bool>> predicate, PageReq req,
        params Expression<Func<TEntity, object>>[] includeProperties) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllIncludingQueryable(false, includeProperties);
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;
        var data = (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync());
        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode),
        };
    }

    private async Task<SourcePaged<TEntity>> GetSourcePagedAsync(IQueryable<TEntity> source, PageReq req)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync(),
        };
    }

    private async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(IQueryable<TEntity> source, PageReq req)
        where TDto : class, IBaseDto<TPrimary>
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()).Select(_mapper.Map<TDto>).ToList(),
        };
    }

    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, bool isDeleted = false)
    {
        if (req == null) throw new ArgumentNullException(nameof(req) + "is null");
        var source = GetAllQueryable(isDeleted);
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync(),
        };
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, bool isDeleted = false) where TDto : class, IBaseDto<TPrimary>
    {
        if (req == null) throw new ArgumentNullException(nameof(req) + " is null");
        var source = GetAllQueryable(isDeleted);
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;

        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = (await Paginate<TEntity>.Paging(source, req.Page).ToListAsync()).Select(_mapper.Map<TDto>).ToList(),
        };
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req, bool isDeleted = false) where TDto : class, IBaseDto<TPrimary>
    {
        if (req == null) throw new ArgumentNullException(nameof(req) + " is null");
        var source = GetAllQueryable(isDeleted);
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;
        var data = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync();
        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
        };
    }

    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null)
    {
        var source = GetAllQueryable();
        if (func != null)
            source = func(source);

        return await GetSourcePagedAsync(source, req);
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        if (func != null)
            source = func(source);
        return await GetSourcePagedAsync<TDto>(source, req);
    }

    public virtual async Task<SourcePaged<TDto>> GetSourcePagedAsync<TDto>(string languageCode, PageReq req, Func<IQueryable<TEntity>, IQueryable<TEntity>>? func = null) where TDto : class, IBaseDto<TPrimary>
    {
        var source = GetAllQueryable();
        if (func != null)
            source = func(source);
        req.Page = req.Page == 0 ? 1 : req.Page;

        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;
        var data = await Paginate<TEntity>.Paging(source, req.Page).ToListAsync();
        return new SourcePaged<TDto>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = LanguageHelper.GetLocalizedList<TEntity, TDto, TPrimary>(data, data.Select(_mapper.Map<TDto>).ToList(), languageCode)
        };
    }

    public virtual async Task<SourcePaged<TEntity>> GetSourcePagedAsync(PageReq req, Func<List<TEntity>, List<TEntity>>? func = null)
    {
        var source = GetAllQueryable();
        req.Page = req.Page == 0 ? 1 : req.Page;
        // PerPage Count
        if (req.PerPage > 0 && req.PerPage <= 200)
            Paginate<TEntity>.PerPage = req.PerPage;
        return new SourcePaged<TEntity>
        {
            PagingModel = new PageModel(await source.CountAsync(), req.Page, Paginate<TEntity>.PerPage),
            Source = func == null ? await Paginate<TEntity>.Paging(source, req.Page).ToListAsync() : func((await Paginate<TEntity>.Paging(source, req.Page).ToListAsync())),
        };
    }

    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().AnyAsync(predicate);

    public virtual Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().AllAsync(predicate);

    public virtual Task<int> CountAsync()
        => GetAllQueryable().CountAsync();

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        => GetAllQueryable().CountAsync(predicate);
}