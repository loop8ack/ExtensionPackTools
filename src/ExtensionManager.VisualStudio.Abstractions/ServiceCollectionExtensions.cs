using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.VisualStudio;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureVSServices(this IServiceCollection services, IVSServicesRegistrar registrar)
    {
        registrar.AddServices(services);
        return services;
    }
}
