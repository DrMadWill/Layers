using System.Reflection;
using AutoMapper;
using DrMadWill.Layers.Abstractions.Repository.Repositories.CQRS;
using DrMadWill.Layers.Abstractions.Service;

namespace DrMadWill.Layers.Concrete.Service;

public abstract class ServiceManager : IServiceManager
{
    private readonly Dictionary<Type, object> _services;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueryRepositories _queryRepositories;
    private readonly IMapper _mapper;
    protected readonly Type _assembly;
    public ServiceManager(IUnitOfWork unitOfWork, IQueryRepositories queryRepositories, IMapper mapper,Type type)
    {
        _unitOfWork = unitOfWork;
        _queryRepositories = queryRepositories;
        _mapper = mapper;
        _services = new Dictionary<Type, object>();
        _assembly = type;
    }
   
    
    public virtual TService Service<TService>()
    {
        if (_services.Keys.Contains(typeof(TService)))
            return (TService)_services[typeof(TService)];
        
        var type = _assembly.Assembly.GetTypes()
            .FirstOrDefault(x => !x.IsAbstract
                                 && !x.IsInterface
                                 && x.Name == typeof(TService).Name.Substring(1));;

        if (type == null)
            throw new KeyNotFoundException($"Service type is not found. Service Name { typeof(TService).Name.Substring(1)}");

        var service = (TService)Activator.CreateInstance(type, _unitOfWork,_queryRepositories,_mapper)!;

        _services.Add(typeof(TService), service);

        return service;
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
            

            // _services clear 
            foreach (var service in _services.Values)
                if (service is IDisposable disposableRepo)
                    disposableRepo.Dispose();
            
            _services.Clear();
        }

    }

    ~ServiceManager()
    {
        Dispose(false);
    }
}