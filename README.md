
# Layers

## Description

Layers is a software project developed primarily in C#.

## Installation

To install Layers, you will need .
1. You can clone the repository using the following command:

    - bash
        - Copy code `git clone https://github.com/DrMadWill/Layers.git`
2. You can install the package form your project using the following command:
    - nuget
        - terminal>  `dotnet add package DrMadWill.Layers --version 3.1.0`
        - nuget-terminal> `NuGet\Install-Package DrMadWill.Layers -Version 3.1.0`


## Usage

Here is how you can use Layers:
This class must be added your project.
```cs

public static class ServiceRegistration  
{  
    public static void AddLayerServices(this IServiceCollection services)  
    {  
        if (services == null) throw new AggregateException("service is null");  
  
        #region Repositories  
        services.AddScoped(typeof(IReadRepository<,>), typeof(ReadRepository<,>));  
        services.AddScoped(typeof(IWriteRepository<,>), typeof(WriteRepository<,>));  
        services.AddScoped<IQueryRepositories, DQueryRepositories>();   
        services.AddScoped<IUnitOfWork, DUnitOfWork>();  
        #endregion  
  
		#region Services  
        services.AddScoped<IServiceManager, DServiceManager>();    
        #endregion  
  
  }  
}
```

- DQueryRepositories
```cs

public class DQueryRepositories : QueryRepositories, IQueryRepositories  
{  
    public DQueryRepositories(DictContext dictContext, IMapper mapper) : base(dictContext, mapper, typeof(ServiceRegistration))  
    {  
    }  
}
```
- DUnitOfWork
```cs
public class DUnitOfWork:UnitOfWork  
{  
    public DUnitOfWork(DictContext dictContext) : base(dictContext,typeof(ServiceRegistration))  
    {  
    }  
}
```
- DServiceManager
```cs
public class DServiceManager :ServiceManager  
{  
    public DServiceManager(IUnitOfWork unitOfWork, IQueryRepositories queryRepositories, IMapper mapper,ILoggerFactory loggerFactory) :   
        base(unitOfWork, queryRepositories, mapper,loggerFactory,typeof(ServiceRegistration))  
    {  
    }  
      
}
```

For example : Dictionary Type Service Add
```cs
public class DictionaryTypeService:BaseService,IDictionaryTypeService  
{  
    private readonly IReadRepository<DictionaryType, int> _read;  
    private readonly IWriteRepository<DictionaryType, int> _write;  
    public DictionaryTypeService(IUnitOfWork unitOfWork, IQueryRepositories queryRepositories,IMapper mapper,ILogger<DictionaryTypeService> logger) :   
        base(unitOfWork, queryRepositories,mapper,logger)  
    {  
        _read = QueryRepositories.Repository<DictionaryType,int>();  
        _write = UnitOfWork.Repository<DictionaryType,int>();  
    }  
      
    public async Task<SourcePaged<DictionaryTypeDto>> GetAll(DictionaryTypeFilter filter,PageReq page)  
    {  
        IQueryable<DictionaryType> query = QueryRepositories.Repository<DictionaryType, int>().GetAllQueryable()  
            .OrderByDescending(s => s.Hierarchy);  
        if (filter != null)  
            query = filter.Filtered(query);  
        var source =  
            await SourcePaged<DictionaryTypeDto>.PagedAsync(query, page);  
  
        return Mapper.Map<SourcePaged<DictionaryTypeDto>>(source);  
    }  
      
    public async Task<DictionaryTypeDto> Get(int id)  
    {  
        var dictionaryType = await _read.GetFirstAsync(s => s.Id == id);  
        if (dictionaryType == null) return null;  
        var result = Mapper.Map<DictionaryTypeDto>(dictionaryType);  
        return result;  
    }  
  
    public Task<bool> AddAsync(AddDictionaryTypeReq req)  
        => CompleteProcess(async () =>  
        {  
            var dictionaryType = Mapper.Map<DictionaryType>(req);  
            await _write.AddAsync(dictionaryType);  
            await UnitOfWork.CommitAsync();  
        });  
     
    public Task<bool> UpdateAsync(UpdateDictionaryTypeReq req)  
        => CompleteProcess(async () =>  
        {  
            var dictionaryType = await _read.GetAllQueryable(true).FirstOrDefaultAsync(s => s.Id == req.Id);  
            if (dictionaryType == null) throw new Exception("Dictionary Type is not found");  
            Mapper.Map(req, dictionaryType);  
            await _write.UpdateAsync(dictionaryType);  
            await UnitOfWork.CommitAsync();  
        });  
  
    public Task<bool> DeleteAsync(int id)  
       => CompleteProcess(async () =>  
       {  
            var dictionaryType =await _read.GetAllQueryable(true).FirstOrDefaultAsync(s => s.Id == id);  
            if (dictionaryType == null) throw new Exception("Dictionary Type is not found");   
            await _write.DeleteAsync(dictionaryType);  
            await UnitOfWork.CommitAsync();   
       });  
      
}
```

## Contributing

Contributions to Layers are welcome! Please feel free to fork the repository, make changes, and submit pull requests.
## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/DrMadWill/Layers/blob/main/LICENSE) file for details.

## Acknowledgments

-   me