using System.Collections.Generic;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using ExtensionManager.VisualStudio.Extensions;

using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public sealed class VSExtensionRepositoryAdapter : IVSExtensionRepositoryAdapter<IVsExtensionRepository>
{
    public Task<IVsExtensionRepository> GetRepositoryAsync()
        => VS.GetRequiredServiceAsync<SVsExtensionRepository, IVsExtensionRepository>();

    public IEnumerable<IVSExtension> GetVSGalleryExtensions(IVsExtensionRepository repository, List<string> extensionIds, int lcid, bool forAutoupdate)
        => repository.GetVSGalleryExtensions<GalleryExtension>(extensionIds, lcid, forAutoupdate);
}
