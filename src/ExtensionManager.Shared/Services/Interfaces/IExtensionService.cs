using System.Collections.Generic;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of an object that allows
    /// access to the list of installed extensions.
    /// </summary>
    public interface IExtensionService
    {
        /// <summary>
        /// Obtains a list of all the extensions installed in this instance of Visual
        /// Studio that were obtained from the Visual Studio Marketplace.
        /// </summary>
        /// <returns>
        /// Collection of instances of objects that implement the
        /// <see cref="T:ExtensionManager.IExtension" />
        /// , one for each of the extensions that are installed, which are initialized with
        /// the extension metadata.
        /// <para />
        /// If no extensions obtained from the Visual Studio Marketplace are installed, or
        /// if an error occurs, then the empty collection is returned.
        /// </returns>
        IEnumerable<IExtension> GetInstalledExtensions();
    }
}