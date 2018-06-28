using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExtensionManager
{
    public static class Helpers
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

        public static BitmapSource GetIconForImageMoniker(ImageMoniker imageMoniker, int sizeX, int sizeY)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(ServiceProvider.GlobalProvider.GetService(typeof(SVsImageService)) is IVsImageService2 vsIconService))
            {
                return null;
            }

            var imageAttributes = new ImageAttributes
            {
                Flags = (uint)_ImageAttributesFlags.IAF_RequiredFlags,
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WPF,
                LogicalHeight = sizeY,
                LogicalWidth = sizeX,
                StructSize = Marshal.SizeOf(typeof(ImageAttributes))
            };

            IVsUIObject result = vsIconService.GetImage(imageMoniker, imageAttributes);

            result.get_Data(out object data);
            var glyph = data as BitmapSource;

            if (glyph != null)
            {
                glyph.Freeze();
            }

            return glyph;
        }
    }
}
