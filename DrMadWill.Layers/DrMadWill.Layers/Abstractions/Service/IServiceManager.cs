namespace DrMadWill.Layers.Abstractions.Service;
/// <summary>
/// Base class for managing services.
/// </summary>
public interface IServiceManager : IDisposable
{
    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of service to retrieve.</typeparam>
    /// <returns>An instance of the specified service type.</returns>
    TService Service<TService>();
}