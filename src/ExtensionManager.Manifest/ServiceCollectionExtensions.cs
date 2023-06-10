using ExtensionManager.Manifest.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.Manifest;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddManifestService(this IServiceCollection services)
    {
        services.AddTransient<IManifestService, ManifestService>();

        return services;
    }
}
