using Newtonsoft.Json;

namespace ExtensionManager
{
    public class Extension
    {
        [JsonProperty("downloadUrl")] public string DownloadUrl { get; set; }

        [JsonProperty("vsixId")] public string ID { get; set; }

        [JsonProperty("moreInfoUrl")] public string MoreInfoUrl { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonIgnore] public bool Selected { get; set; }

        public static Extension FromGalleryEntry(GalleryEntry entry)
        {
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