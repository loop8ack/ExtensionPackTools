using ExtensionManager.Features.Export;
using ExtensionManager.Features.Install;
using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;

using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureExtensionManager(this IServiceCollection services)
    {
        return services
            .AddDialogService()
            .AddManifestService()
            .AddTransient<ExportFeatureBase.Args>()
            .AddTransient<InstallFeatureBase.Args>()
            .AddTransient<IFeatureExecutor, FeatureExecutor>()
            .AddTransient<IExtensionInstaller, ExtensionInstaller>();
    }
}
