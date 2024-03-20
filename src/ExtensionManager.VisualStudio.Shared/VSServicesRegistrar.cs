using System;

using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

using Microsoft.Extensions.DependencyInjection;

#if V15
namespace ExtensionManager.VisualStudio.V15;
#elif V16
namespace ExtensionManager.VisualStudio.V16;
#elif V17
namespace ExtensionManager.VisualStudio.V17;
#endif

public sealed class VSServicesRegistrar : IVSServicesRegistrar
{
    private readonly Version _visualStudioVersion;

    public VSServicesRegistrar(Version visualStudioVersion)
        => _visualStudioVersion = visualStudioVersion;

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton(new VSAdapterServicesFactory(_visualStudioVersion));
        services.AddSingleton(s => s.GetRequiredService<VSAdapterServicesFactory>().CreateExtensionManagerAdapter());
        services.AddSingleton(s => s.GetRequiredService<VSAdapterServicesFactory>().CreateExtensionRepositoryAdapter());

        services.AddSingleton<IVSThemes, VSThemes>();
        services.AddSingleton<IVSThreads, VSThreads>();
        services.AddSingleton<IVSSolutions, VSSolutions>();
        services.AddSingleton<IVSStatusBar, VSStatusBar>();
        services.AddSingleton<IVSDocuments, VSDocuments>();
        services.AddSingleton<IVSMessageBox, VSMessageBox>();
        services.AddSingleton<IVSExtensions, VSExtensions>();
    }
}
