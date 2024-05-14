using DrMadWill.Layers.Repository.Abstractions.CQRS;
using DrMadWill.Layers.Repository.Abstractions.Sys;
using DrMadWill.Layers.Repository.Concretes.CQRS;
using DrMadWill.Layers.Repository.Concretes.Sys;
using Microsoft.Extensions.DependencyInjection;

namespace DrMadWill.Layers.Repository;

public static class ServiceRegistration
{
    public static IServiceCollection LayerRepositoriesRegister<TUnitOfWork,TQueryRepositories>
        (this IServiceCollection services,ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TUnitOfWork : UnitOfWork
        where TQueryRepositories : QueryRepositories
    {
        RegisterService(typeof(IReadRepository<,>), typeof(ReadRepository<,>));
        RegisterService(typeof(IWriteRepository<,>), typeof(WriteRepository<,>));
        RegisterService(typeof(IWriteOriginRepository<,>), typeof(WriteOriginRepository<,>));
        RegisterService(typeof(IReadOriginRepository<,>), typeof(ReadOriginRepository<,>));
        RegisterService(typeof(IAnonymousRepository<>), typeof(AnonymousRepository<>));

        services.Add(new ServiceDescriptor(typeof(IQueryRepositories), typeof(TQueryRepositories), lifetime));
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(TUnitOfWork), lifetime));

        return services;

        void RegisterService(Type serviceType, Type implementationType)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), $"Unsupported service lifetime: {lifetime}");
            }
        }
    }
}