using System.Collections.Generic;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a service that is
    /// responsible for fetching metadata for installed Visual Studio extensions by
    /// looking that metadata up using, e.g., their identifiers
    /// .
    /// </summary>
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
        IEnumerable<IExtension> GetExtensionMetadata(IList<string> identifiers);
    }
}