using System;

using ExtensionManager.VisualStudio.Adapter;
using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

using Microsoft.Extensions.DependencyInjection;

#nullable enable

#if VS2017
namespace ExtensionManager.VisualStudio.VS2017;
#elif VS2019
namespace ExtensionManager.VisualStudio.VS2019;
#elif VS2022
namespace ExtensionManager.VisualStudio.VS2022;
#else
#error Not implemented
#endif

public sealed class VSServicesRegistrar : IVSServicesRegistrar
{
    private readonly Version _visualStudioVersion;

    public VSServicesRegistrar(Version visualStudioVersion)
        => _visualStudioVersion = visualStudioVersion;

    public void AddServices(IServiceCollection services)
    {
        services.AddSingleton(new VSAdapterServicesFactoryGenerator(_visualStudioVersion).Generate());
        services.AddSingleton(s => s.GetRequiredService<IVSAdapterServicesFactory>().CreateExtensionManagerAdapter());
        services.AddSingleton(s => s.GetRequiredService<IVSAdapterServicesFactory>().CreateExtensionRepositoryAdapter());

        services.AddSingleton<IVSThemes, VSThemes>();
        services.AddSingleton<IVSThreads, VSThreads>();
        services.AddSingleton<IVSSolutions, VSSolutions>();
        services.AddSingleton<IVSStatusBar, VSStatusBar>();
        services.AddSingleton<IVSDocuments, VSDocuments>();
        services.AddSingleton<IVSMessageBox, VSMessageBox>();
        services.AddSingleton<IVSExtensions, VSExtensions>();
    }
}
