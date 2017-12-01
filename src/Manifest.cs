using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtensionPackTools
{
    public class Manifest
    {
        public Manifest(IEnumerable<GalleryEntry> entries)
        {
            Extensions = entries.Select(e => new Extension
            {
                ID = e.VsixID,
                Name = e.Name,
            });
        }

        [JsonProperty("id")]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("name")]
        public string Name { get; set; } = "My Visual Studio extensions";

        [JsonProperty("description")]
        public string Description { get; set; } = "A collection of my Visual Studio extensions";

        [JsonProperty("version")]
        public string Version { get; set; } = "1.0";

        [JsonProperty("extensions")]
        public IEnumerable<Extension> Extensions { get; set; }

        public class Extension
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("vsixId")]
            public string ID { get; set; }
        }
    }
}
