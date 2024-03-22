using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.VisualStudio;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> to facilitate the configuration of services related to Visual Studio.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures services for interacting with Visual Studio by utilizing a specified <see cref="IVSServicesRegistrar"/>.
    /// </summary>
    public static IServiceCollection ConfigureVSServices(this IServiceCollection services, IVSServicesRegistrar registrar)
    {
        registrar.AddServices(services);
        return services;
    }
}
