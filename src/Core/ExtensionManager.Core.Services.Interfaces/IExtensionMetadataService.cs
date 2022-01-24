using System.Collections.Generic;
using ExtensionManager.Core.Models.Interfaces;

namespace ExtensionManager
{
    public interface IExtensionMetadataService
    {
        /// <summary>
        /// Returns the collection of extension metadata items, given a collection of
        /// strings containing the identifiers of the extensions for which the metadata is
        /// being requested.
        /// </summary>
        /// <param name="identifiers">
        /// (Required.) Collection of strings containing the
        /// identifiers of the extensions for which the metadata is being requested.
        /// </param>
        /// <returns>
        /// An enumerable collection, sorted by human-readable name
        /// alphabetically, of the extension metadata objects.
        /// <para />
        /// If an error occurs or the operation otherwise cannot be carried out, then this
        /// method returns the empty collection.
        /// </returns>
        IEnumerable<IExtension> GetExtensionMetadata(
            List<string> identifiers);
    }
}