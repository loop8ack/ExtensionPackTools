namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public interface IVSInstalledExtensionInfo
{
    string Identifier { get; }
    bool IsSystemComponent { get; }
    bool IsPackComponent { get; }
}
