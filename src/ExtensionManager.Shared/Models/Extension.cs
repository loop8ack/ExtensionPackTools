using Microsoft.VisualStudio.ExtensionManager;
using Newtonsoft.Json;

namespace ExtensionManager
{
    public class Extension
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vsixId")]
        public string ID { get; set; }

        [JsonProperty("moreInfoUrl")]
        public string MoreInfoUrl { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }

        public static Extension FromGalleryExtension(GalleryEntry entry)
        {
            return new Extension { ID = entry.VsixID, Name = entry.Name, MoreInfoUrl = entry.MoreInfoURL, DownloadUrl = entry.DownloadUrl };
        }

        public static Extension FromIExtension(IExtension entry)
        {
            return new Extension { ID = entry.Header.Identifier, Name = entry.Header.Name, MoreInfoUrl = entry.Header.MoreInfoUrl?.ToString()};
        }

        public override bool Equals(object obj)
        {
            return !(obj is Extension other) ? false : ID == other.ID;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
