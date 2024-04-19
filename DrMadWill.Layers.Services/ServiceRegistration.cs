using DrMadWill.Layers.Services.Abstractions;
using DrMadWill.Layers.Services.Concretes;
using Microsoft.Extensions.DependencyInjection;

namespace DrMadWill.Layers.Services;

public static class ServiceRegistration
{
    public static IServiceCollection LayerServicesRegister<TServiceManager>
        (this IServiceCollection services,ServiceLifetime lifetime = ServiceLifetime.Scoped,bool hasSessionManager = true)
        where TServiceManager : ServiceManager
      
    {
        services.Add(new ServiceDescriptor(typeof(IServiceManager), typeof(TServiceManager), lifetime));
        if(hasSessionManager) services.AddScoped<ISessionManger, SessionManager>();
        return services;
    }
}