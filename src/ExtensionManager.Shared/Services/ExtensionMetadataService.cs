using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using IExtension = ExtensionManager.Core.Models.Interfaces.IExtension;

namespace ExtensionManager
{
    /// <summary>
    /// Service to provide metadata on installed extensions.
    /// </summary>
    /// <remarks>
    /// This service only provides metadata on those extensions for whom the
    /// identifiers are provided.
    /// </remarks>
    public class ExtensionMetadataService : IExtensionMetadataService
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository" />
        /// interface.
        /// </summary>
        private readonly IVsExtensionRepository _repository;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.ExtensionMetadataService" /> and returns a
        /// reference to it.
        /// </summary>
        public ExtensionMetadataService(IVsExtensionRepository repository)
        {
            _repository = repository ??
                          throw new ArgumentNullException(nameof(repository));
        }

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
        public IEnumerable<IExtension> GetExtensionMetadata(
            List<string> identifiers)
        {
            var result = Enumerable.Empty<IExtension>()
                                   .OrderBy(x => 1);

            if (!identifiers.Any())
                return result;  // oops, do not have any search terms, are zero extensions installed by the user?

            try
            {
                result = _repository
                         .GetVSGalleryExtensions<GalleryEntry>(
                             identifiers, 1033, false
                         )
                         .Select(MakeNewExtension.FromGalleryEntry)
                         .OrderBy(e => e.Name);
            }
            catch (Exception ex)
            {
                // dump all the exception info to the log
                Debug.WriteLine(ex);

                result = Enumerable.Empty<IExtension>()
                                   .OrderBy(x => 1);
            }

            return result;
        }
    }
}