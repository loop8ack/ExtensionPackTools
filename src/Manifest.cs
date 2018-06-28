using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ExtensionPackTools
{
    public class Manifest
    {
        public Manifest() : this(new List<Extension>())
        { }

        public Manifest(IEnumerable<Extension> entries)
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
        public IEnumerable<Extension> Extensions { get; set; }

        public static Manifest FromFile(string filePath)
        {
            string file = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Manifest>(file);
        }
    }
}
