using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ExtensionManager
{
    public class Manifest
    {
        public Manifest() : this(new List<IExtension>()) { }

        public Manifest(IEnumerable<IExtension> entries)
        {
            Extensions = entries;
        }

        [JsonProperty("id")]
        public string ID { get; set; } = Guid.NewGuid()
                                             .ToString();

        [JsonProperty("name")]
        public string Name { get; set; } = "My Visual Studio extensions";

        [JsonProperty("description")]
        public string Description { get; set; } =
            "A collection of my Visual Studio extensions";

        [JsonProperty("version")] public string Version { get; set; } = "1.0";

        [JsonProperty("extensions")]
        public IEnumerable<IExtension> Extensions { get; set; }

        /// <summary>
        /// Marks the extensions specified in the <paramref name="installed" /> list as
        /// selected.
        /// </summary>
        /// <param name="installed">
        /// (Required.) Reference to an enumerable collection of
        /// objects that implement the <see cref="T:ExtensionManager.IExtension" />
        /// interface.
        /// </param>
        /// <remarks>
        /// If there are zero elements in the <paramref name="installed" />
        /// collection, or if the argument of the <paramref name="installed" /> parameter
        /// is a <see langword="null" /> reference, then this method does nothing.
        /// <para />
        /// Otherwise, this method marks those extensions that are in the
        /// <paramref name="installed" /> collection as selected.
        /// <para />
        /// The idea is to indicate to the user which extensions, among those being
        /// exported or imported, are currently installed in Visual Studio.
        /// </remarks>
        public void MarkAlreadyInstalledExtensionsAsSelected(
            IList<IExtension> installed)
        {
            if (installed == null) return;
            if (!installed.Any()) return;

            foreach (var extension in Extensions)
            {
                var ext = (Extension)extension;
                ext.Selected = !installed.Contains(ext);
            }
        }
    }
}