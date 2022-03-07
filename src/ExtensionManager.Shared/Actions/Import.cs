using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExtensionManager
{
    public interface IManifestImporter
    {
        /// <summary>
        /// Imports a <see cref="T:ExtensionManager.Manifest" /> object from the file whose
        /// <paramref name="pathname" /> is specified.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the file that is to be imported.
        /// </param>
        /// <returns>
        /// If successful, a reference to an instance of
        /// <see cref="T:ExtensionManager.Manifest" /> that contains the data imported.  If
        /// the operation failed, then a <see langword="null" /> reference is returned.
        /// </returns>
        Manifest FromFile(string pathname);
    }

    public class Import : IManifestImporter
    {
        /// <summary>
        /// Empty, static constructor to prohibit direct allocation of this class.
        /// </summary>
        static Import() { }

        /// <summary>
        /// Empty, protected constructor to prohibit direct allocation of this class.
        /// </summary>
        protected Import() { }

        /// <summary>
        /// Gets a reference to the one and only instance of
        /// <see cref="T:ExtensionManager.Import" />.
        /// </summary>
        public static IManifestImporter Manifest { get; } = new Import();

        /// <summary>
        /// Imports a <see cref="T:ExtensionManager.Manifest" /> object from the file whose
        /// <paramref name="pathname" /> is specified.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the file that is to be imported.
        /// </param>
        /// <returns>
        /// If successful, a reference to an instance of
        /// <see cref="T:ExtensionManager.Manifest" /> that contains the data imported.  If
        /// the operation failed, then a <see langword="null" /> reference is returned.
        /// </returns>
        public Manifest FromFile(string pathname)
        {
            /*
             * Really should handle potential I/O exceptions here.
             *
             * As well as check that the file whose pathname is
             * specified in the filePath argument really exists
             * on the disk in the first place.
             */

            var file = File.ReadAllText(pathname);

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

                var extensions = mandatory.Union(optional)
                                          .Cast<JObject>();

                var list = new List<IExtension>();

                foreach (var child in extensions)
                {
                    var ext = new Extension {
                        ID = child["productId"]
                            .Value<string>(),
                        Name = child["name"]
                            .Value<string>(),
                        MoreInfoUrl = child["link"]
                            .Value<string>()
                    };

                    list.Add(ext);
                }

                var manifest = new Manifest(list) {
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