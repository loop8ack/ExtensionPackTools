using System.Collections.Generic;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a service whose
    /// responsibility it is to obtain the identifiers of currently-installed Visual
    /// Studio Extensions.
    /// </summary>
    /// <remarks>
    /// This class is in charge of getting the identifiers of those extensions
    /// that are (a) obtained from the Visual Studio Marketplace, (b) are not system
    /// components, and (c) is not a component of a package (<c>IsPackComponent</c> is
    /// <see langword="false" />).
    /// </remarks>
    public interface IExtensionIdentifierService
    {
        /// <summary>
        /// Attempts to obtain a collection of strings that contains the set of identifiers
        /// for the installed extensions.
        /// <para />
        /// This method only searches for extensions that have been obtained from the
        /// Visual Studio Marketplace, are not system components, and are not components of
        /// packages.
        /// </summary>
        /// <returns>
        /// Collection of strings that contains the set of identifiers for the
        /// installed extensions.
        /// <para />
        /// This method only searches for extensions that have been obtained from the
        /// Visual Studio Marketplace, are not system components, and are not components of
        /// packages.
        /// <para />
        /// If the operation fails, or if no installed extensions can be found, then this
        /// method returns the empty collection.
        /// </returns>
        IEnumerable<string> GetInstalledExtensionIdentifiers();
    }
}