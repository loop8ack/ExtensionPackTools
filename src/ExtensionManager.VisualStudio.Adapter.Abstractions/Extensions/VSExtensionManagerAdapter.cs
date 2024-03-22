namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public sealed class VSExtensionManagerAdapter<TManager, TInstalledExtension> : IVSExtensionManagerAdapter
    where TManager : class
    where TInstalledExtension : class
{
    private readonly IVSExtensionManagerAdapter<TManager, TInstalledExtension> _genericAdapter;

    public VSExtensionManagerAdapter(IVSExtensionManagerAdapter<TManager, TInstalledExtension> genericAdapter)
        => _genericAdapter = genericAdapter;

    public async Task<IReadOnlyList<IVSInstalledExtensionInfo>> GetInstalledExtensionsAsync()
    {
        var manager = await _genericAdapter.GetManagerAsync();

        return _genericAdapter.GetInstalledExtensions(manager)
            .Select(_genericAdapter.CreateInstalledExtensionInfo)
            .ToList();
    }
}
