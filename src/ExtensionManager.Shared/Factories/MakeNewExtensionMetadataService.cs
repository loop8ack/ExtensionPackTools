using System;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Creates new instances of objects that implement the
    /// <see cref="T:ExtensionManager.IExtensionMetadataService" /> interface, and
    /// returns references to them.
    /// </summary>
    public static class MakeNewExtensionMetadataService
    {
        /// <summary>
        /// Creates a new instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionMetadataService" /> interface and
        /// returns a reference to it.
        /// </summary>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionMetadataService" /> interface.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="repository" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        public static IExtensionMetadataService ForVsExtensionRepository(
            IVsExtensionRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            return new ExtensionMetadataService(repository);
        }
    }
}