using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtensionManager
{
    public class Manifest
    {
        public Manifest() : this(new List<IExtension>())
        { }

        public Manifest(IEnumerable<IExtension> entries)
        {
            Extensions = entries;
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
        public IEnumerable<IExtension> Extensions { get; set; }

        public void MarkSelected(IEnumerable<IExtension> installed)
        {
            foreach (var extension in Extensions)
            {
                var ext = (Extension)extension;
                ext.Selected = !installed.Contains(ext);
            }
        }
    }
}
