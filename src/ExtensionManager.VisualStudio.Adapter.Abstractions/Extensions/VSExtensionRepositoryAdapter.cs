using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public sealed class VSExtensionRepositoryAdapter<TRepository> : IVSExtensionRepositoryAdapter
    where TRepository : class
{
    private readonly IVSExtensionRepositoryAdapter<TRepository> _genericAdapter;

    public VSExtensionRepositoryAdapter(IVSExtensionRepositoryAdapter<TRepository> genericAdapter)
        => _genericAdapter = genericAdapter;

    public async Task<IReadOnlyList<IVSExtension>> GetVSGalleryExtensionsAsync(List<string> extensionIds, int lcid, bool forAutoupdate)
    {
        var repository = await _genericAdapter.GetRepositoryAsync();

        return _genericAdapter
            .GetVSGalleryExtensions(repository, extensionIds, lcid, forAutoupdate)
            .ToList();
    }
}
