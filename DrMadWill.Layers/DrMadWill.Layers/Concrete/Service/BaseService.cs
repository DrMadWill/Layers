using DrMadWill.Layers.Abstractions.Repository.CQRS;
using DrMadWill.Layers.Abstractions.Service;

namespace DrMadWill.Layers.Concrete.Service;

public abstract class  BaseService : IBaseService
{
    protected readonly IUnitOfWork UnitOfWork;
    protected readonly IQueryRepositories QueryRepositories;

    protected BaseService(IUnitOfWork unitOfWork, IQueryRepositories queryRepositories)
    {
        UnitOfWork = unitOfWork;
        QueryRepositories = queryRepositories;
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
            UnitOfWork.Dispose();
            QueryRepositories.Dispose();
        }
    }
        
    ~BaseService()
    {
        Dispose(false);
    }
}