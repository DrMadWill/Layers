namespace DrMadWill.Layers.Abstractions.Service;
public interface IServiceManager : IDisposable
{
    TRepository Service<TRepository>();
}