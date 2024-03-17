using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

internal sealed class InstalledExtensionInfo : IVSInstalledExtensionInfo
{
    private readonly IInstalledExtension _extension;

    public string Identifier => _extension.Header.Identifier;
    public bool IsSystemComponent => _extension.Header.SystemComponent;
    public bool IsPackComponent => _extension.IsPackComponent;

    public InstalledExtensionInfo(IInstalledExtension extension)
        => _extension = extension;
}
