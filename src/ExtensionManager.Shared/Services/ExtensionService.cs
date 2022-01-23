using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExtensionManager.Core.Services.Interfaces;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using IExtension = ExtensionManager.Core.Models.Interfaces.IExtension;

namespace ExtensionManager
{
    /// <summary>
    /// Service that provides access to extension metadata from the Visual Studio
    /// Marketplace.
    /// </summary>
    public class ExtensionService : IExtensionService
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionIdentifierService" /> interface.
        /// </summary>
        private readonly IExtensionIdentifierService
            _extensionIdentifierService;

        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionMetadataService" /> interface.
        /// </summary>
        private readonly IExtensionMetadataService _extensionMetadataService;

        /// <summary>
        /// Constructs a new instance of <see cref="T:ExtensionManager.ExtensionService" />
        /// and returns a reference to it.
        /// </summary>
        /// <param name="manager">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </param>
        /// <param name="repository">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository" />
        /// interface.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if either of the
        /// required parameters, <paramref name="manager" />, or
        /// <paramref name="repository" />, are passed a <see langword="null" /> value.
        /// </exception>
        public ExtensionService(IVsExtensionManager manager,
            IVsExtensionRepository repository)
        {
            _extensionIdentifierService =
                MakeNewExtensionIdentifierService
                    .ForVsExtensionManager(manager);
            _extensionMetadataService =
                MakeNewExtensionMetadataService.ForVsExtensionRepository(
                    repository
                );
        }

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
        public IEnumerable<IExtension> GetInstalledExtensions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            List<IExtension> result;

            try
            {
                // Filter the installed extensions to only be the ones that exist on the Marketplace

                result = _extensionMetadataService.GetExtensionMetadata(
                                                      _extensionIdentifierService
                                                          .GetInstalledExtensionIdentifiers()
                                                          .ToList()
                                                  )
                                                  .ToList();
            }
            catch (Exception ex)
            {
                // dump all the exception info to the Output window of the debugger
                Debug.WriteLine(ex);

                // Reinitialize the result to the empty collection
                result = new List<IExtension>();
            }

            return result;
        }
    }
}