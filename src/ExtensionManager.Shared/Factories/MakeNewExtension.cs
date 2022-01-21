using System;
using ExtensionManager.Core.Models.Interfaces;

namespace ExtensionManager
{
    public static class MakeNewExtension
    {
        public static IExtension FromGalleryEntry(IGalleryEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return new Extension {
                ID = entry.VsixID,
                Name = entry.Name,
                MoreInfoUrl = entry.MoreInfoURL,
                DownloadUrl = entry.DownloadUrl
            };
        }
    }
}