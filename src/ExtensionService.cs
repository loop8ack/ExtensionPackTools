using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public class ExtensionService
    {
        private readonly IVsExtensionManager _manager;
        private readonly IVsExtensionRepository _repository;

        public ExtensionService(IVsExtensionManager manager, IVsExtensionRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        public IEnumerable<Extension> GetInstalledExtensions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var installed = _manager.GetInstalledExtensions()
                                       .Where(i => !i.Header.SystemComponent && !i.IsPackComponent)
                                       .Select(i => i.Header.Identifier)
                                       .ToList();

            // Filter the installed extensions to only be the ones that exist on the Marketplace
            IEnumerable<GalleryEntry> marketplaceEntries = _repository.GetVSGalleryExtensions<GalleryEntry>(installed, 1033, false);
            return marketplaceEntries
                .Select(e => Extension.FromGalleryExtension(e))
                .OrderBy(e => e.Name);
        }
    }
}
