using System.Collections.Generic;

namespace ExtensionManager
{
    public interface IVisualStudioService
    {
        IEnumerable<InstalledExtension> GetInstalledExtensions();
        IEnumerable<IGalleryEntry> GetGalleryEntries(List<string> extensionIds);
    }
}
