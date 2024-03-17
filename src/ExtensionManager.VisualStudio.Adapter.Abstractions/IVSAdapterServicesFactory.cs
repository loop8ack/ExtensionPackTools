using ExtensionManager.VisualStudio.Adapter.Extensions;

namespace ExtensionManager.VisualStudio.Adapter;

public interface IVSAdapterServicesFactory
{
    IVSExtensionManagerAdapter CreateExtensionManagerAdapter();
    IVSExtensionRepositoryAdapter CreateExtensionRepositoryAdapter();
}
