using ExtensionManager.VisualStudio.Adapter;
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
#elif V17_Preview
namespace ExtensionManager.VisualStudio.V17_Preview;
#endif

public sealed class VSServicesRegistrar : IVSServicesRegistrar
{
    public void AddServices(IServiceCollection services)
    {
        var adapterServicesFactory = CreateAdapterServicesFactory();
        services.AddSingleton(_ => adapterServicesFactory.CreateExtensionManagerAdapter());
        services.AddSingleton(_ => adapterServicesFactory.CreateExtensionRepositoryAdapter());

        services.AddSingleton<IVSThemes, VSThemes>();
        services.AddSingleton<IVSThreads, VSThreads>();
        services.AddSingleton<IVSSolutions, VSSolutions>();
        services.AddSingleton<IVSStatusBar, VSStatusBar>();
        services.AddSingleton<IVSDocuments, VSDocuments>();
        services.AddSingleton<IVSMessageBox, VSMessageBox>();
        services.AddSingleton<IVSExtensions, VSExtensions>();
    }

    private static IVSAdapterServicesFactory CreateAdapterServicesFactory()
        => new VSAdapterServicesFactory();
}
