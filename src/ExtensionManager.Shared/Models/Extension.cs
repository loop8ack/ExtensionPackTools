using System;
using ExtensionManager.Core.Models.Interfaces;
using Newtonsoft.Json;

namespace ExtensionManager
{
    /// <summary>
    /// Provides information about an extension for export to a JSON-formatted file with a <c>.vsext</c> extension.
    /// </summary>
    public class Extension
    {
        /// <summary>
        /// Gets or sets a string containing the URL from which the extension can be downloaded.
        /// </summary>
        [JsonProperty("downloadUrl")] public string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the extension.
        /// </summary>
        [JsonProperty("vsixId")] public string ID { get; set; }

        /// <summary>
        /// Gets or sets the URL of the page on the Visual Studio Marketplace website for the extension.
        /// </summary>
        [JsonProperty("moreInfoUrl")] public string MoreInfoUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the name of the extension.
        /// </summary>
        [JsonProperty("name")] public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension is selected.
        /// </summary>
        [JsonIgnore] public bool Selected { get; set; }

        public static Extension FromGalleryEntry(IGalleryEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            return new Extension {
                ID = entry.VsixID,
                Name = entry.Name,
                MoreInfoUrl = entry.MoreInfoURL,
                DownloadUrl = entry.DownloadUrl
            };
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current
        /// object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current
        /// object; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Extension other && ID == other.ID;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}