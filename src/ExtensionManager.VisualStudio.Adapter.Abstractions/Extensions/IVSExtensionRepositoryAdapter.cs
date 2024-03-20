using System.Collections.Generic;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Extensions;

public interface IVSExtensionRepositoryAdapter
{
    Task<IReadOnlyList<IVSExtension>> GetVSGalleryExtensionsAsync(List<string> extensionIds, int lcid, bool forAutoupdate);
}


public interface IVSExtensionRepositoryAdapter<TRepository>
{
    Task<TRepository> GetRepositoryAsync();
    IEnumerable<IVSExtension> GetVSGalleryExtensions(TRepository repository, List<string> extensionIds, int lcid, bool forAutoupdate);
}
