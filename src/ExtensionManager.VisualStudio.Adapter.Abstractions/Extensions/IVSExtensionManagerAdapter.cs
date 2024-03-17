using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public interface IVSExtensionManagerAdapter
{
    Task<IReadOnlyList<IVSInstalledExtensionInfo>> GetInstalledExtensionsAsync();
}

public interface IVSExtensionManagerAdapter<TManager, TInstalledExtension>
    where TManager : class
    where TInstalledExtension : class
{
    Task<TManager> GetManagerAsync();
    IEnumerable<TInstalledExtension> GetInstalledExtensions(TManager manager);
    IVSInstalledExtensionInfo CreateInstalledExtensionInfo(TInstalledExtension extension);
}
