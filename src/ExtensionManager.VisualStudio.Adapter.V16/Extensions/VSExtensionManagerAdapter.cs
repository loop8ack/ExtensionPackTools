using System.Collections.Generic;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

internal sealed class VSExtensionManagerAdapter : IVSExtensionManagerAdapter<IVsExtensionManager, IInstalledExtension>
{
    public Task<IVsExtensionManager> GetManagerAsync()
        => VS.GetRequiredServiceAsync<SVsExtensionManager, IVsExtensionManager>();

    public IEnumerable<IInstalledExtension> GetInstalledExtensions(IVsExtensionManager manager)
        => manager.GetInstalledExtensions();

    public IVSInstalledExtensionInfo CreateInstalledExtensionInfo(IInstalledExtension extension)
        => new InstalledExtensionInfo(extension);
}
