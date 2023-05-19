using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public class ExtensionService
    {
        private readonly IVisualStudioService _vsService;

        public ExtensionService(IVisualStudioService vsService)
        {
            _vsService = vsService;
        }

        public IEnumerable<Extension> GetInstalledExtensions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var installed = _vsService.GetInstalledExtensions()
                                       .Where(i => !i.SystemComponent && !i.IsPackComponent)
                                       .Select(i => i.Identifier)
                                       .ToList();

            // Filter the installed extensions to only be the ones that exist on the Marketplace
            IEnumerable<IGalleryEntry> marketplaceEntries = _vsService.GetGalleryEntries(installed);
            return marketplaceEntries
                .Select(e => Extension.FromGalleryExtension(e))
                .OrderBy(e => e.Name);
        }
    }
}
