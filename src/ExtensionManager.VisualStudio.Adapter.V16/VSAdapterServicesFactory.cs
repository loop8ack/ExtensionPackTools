using ExtensionManager.VisualStudio.Adapter.Extensions;

using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager.VisualStudio.Adapter;

public sealed class VSAdapterServicesFactory : IVSAdapterServicesFactory
{
    public IVSExtensionManagerAdapter CreateExtensionManagerAdapter() => new VSExtensionManagerAdapter<IVsExtensionManager, IInstalledExtension>(new VSExtensionManagerAdapter());
    public IVSExtensionRepositoryAdapter CreateExtensionRepositoryAdapter() => new VSExtensionRepositoryAdapter<IVsExtensionRepository>(new VSExtensionRepositoryAdapter());
}
