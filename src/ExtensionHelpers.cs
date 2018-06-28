using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionPackTools
{
    public static class ExtensionHelpers
    {
        public static IEnumerable<Extension> GetInstalledExtensions(IVsExtensionManager manager, IVsExtensionRepository repository)
        {
            var installed = manager.GetInstalledExtensions()
                                       .Where(i => !i.Header.SystemComponent && !i.IsPackComponent && i.State == EnabledState.Enabled)
                                       .Select(i => i.Header.Identifier)
                                       .ToList();

            // Filter the installed extensions to only be the ones that exist on the Marketplace
            IEnumerable<GalleryEntry> marketplaceEntries = repository.GetVSGalleryExtensions<GalleryEntry>(installed, 1033, false);
            return marketplaceEntries
                .Select(e => Extension.FromGalleryExtension(e))
                .OrderBy(e => e.Name);
        }
    }
}
