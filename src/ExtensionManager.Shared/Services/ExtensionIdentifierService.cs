using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes methods to obtain information from external data sources (UI/UX, Visual
    /// Studio, etc.)
    /// </summary>
    /// <remarks>
    /// This class is in charge of getting the identifiers of those extensions
    /// that are (a) obtained from the Visual Studio Marketplace, (b) are not system
    /// components, and (c) is not a component of a package (<c>IsPackComponent</c> is
    /// <see langword="false" />).
    /// </remarks>
    public class ExtensionIdentifierService
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </summary>
        private readonly IVsExtensionManager _manager;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.ExtensionIdentifierService" /> and returns a
        /// reference to it.
        /// </summary>
        /// <param name="manager">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="manager" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        public ExtensionIdentifierService(IVsExtensionManager manager)
        {
            _manager = manager ??
                       throw new ArgumentNullException(nameof(manager));
        }

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
        public IEnumerable<string> GetInstalledExtensionIdentifiers()
        {
            var result = Enumerable.Empty<string>();

            try
            {
                if (_manager == null) return result;

                // Check filtered result for having zero elements come back
                // prior to projecting it.
                if (!_manager.GetInstalledExtensions()
                             .Any(
                                 i => !i.Header.SystemComponent &&
                                      !i.IsPackComponent
                             ))
                    return result;

                result = _manager.GetInstalledExtensions()
                                 .Where(
                                     i => !i.Header.SystemComponent &&
                                          !i.IsPackComponent
                                 )
                                 .Select(i => i.Header.Identifier)
                                 .ToList();
            }
            catch (Exception ex)
            {
                // dump all the exception info to the debugger
                Debug.WriteLine(ex);

                result = Enumerable.Empty<string>();
            }

            return result;
        }
    }
}