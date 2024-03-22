using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.VisualStudio;

/// <summary>
/// Defines a mechanism for registering services for interacting with Visual Studio.
/// </summary>
public interface IVSServicesRegistrar
{
    /// <summary>
    /// Registers services for Visual Studio interaction into the provided service collection.
    /// </summary>
    void AddServices(IServiceCollection services);
}
