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

        public void MarkSelected(IEnumerable<Extension> installed)
        {
            foreach (Extension ext in Extensions)
            {
                ext.Selected = !installed.Contains(ext);
            }
        }

        public static Manifest FromFile(string filePath)
        {
            var file = File.ReadAllText(filePath);

            return From2017Format(file) ?? FromLegacyFormat(file);
        }
        private static Manifest From2017Format(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<Manifest>(json);
            }
            catch
            {
                return null;
            }
        }


        private static Manifest FromLegacyFormat(string json)
        {
            try
            {
                var obj = JObject.Parse(json);
                var mandatory = obj["extensions"]["mandatory"] as JArray;
                var optional = obj["extensions"]["optional"] as JArray;

                IEnumerable<JObject> extensions = mandatory.Union(optional).Cast<JObject>();

                var list = new List<Extension>();

                foreach (JObject child in extensions)
                {
                    var ext = new Extension
                    {
                        ID = child["productId"].Value<string>(),
                        Name = child["name"].Value<string>(),
                        MoreInfoUrl = child["link"].Value<string>(),
                    };

                    list.Add(ext);
                }

                var manifest = new Manifest(list)
                {
                    Name = "Legacy file",
                    ID = Guid.Empty.ToString(),
                    Version = "1.0"
                };

                return manifest;
            }
            catch
            {
                return null;
            }
        }
    }
}
