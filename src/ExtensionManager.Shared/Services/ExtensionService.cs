using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExtensionManager.Core.Models.Factories;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using IExtension = ExtensionManager.Core.Models.Interfaces.IExtension;

namespace ExtensionManager
{
    /// <summary>
    /// Service that provides access to extension metadata from the Visual Studio
    /// Marketplace.
    /// </summary>
    public class ExtensionService
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </summary>
        private readonly IVsExtensionManager _manager;

        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository" />
        /// interface.
        /// </summary>
        private readonly IVsExtensionRepository _repository;

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
        public ExtensionService(IVsExtensionManager manager,
            IVsExtensionRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }

        /// <summary>
        /// Obtains a list of all the extensions installed in this instance of Visual
        /// Studio that were obtained from the Visual Studio Marketplace.
        /// </summary>
        /// <returns>
        /// Collection of instances of objects that implement the <see cref="T:ExtensionManager.IExtension" />
        /// , one for each of the extensions that are installed, which are initialized with
        /// the extension metadata.
        /// <para />
        /// If no extensions obtained from the Visual Studio Marketplace are installed, or
        /// if an error occurs, then the empty collection is returned.
        /// </returns>
        public IEnumerable<IExtension> GetInstalledExtensions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var result = new List<IExtension>();

            try
            {
                var installed = _manager.GetInstalledExtensions()
                                        .Where(
                                            i => !i.Header.SystemComponent &&
                                                 !i.IsPackComponent
                                        )
                                        .Select(i => i.Header.Identifier)
                                        .ToList();

                // Filter the installed extensions to only be the ones that exist on the Marketplace
                var marketplaceEntries =
                    _repository.GetVSGalleryExtensions<GalleryEntry>(
                        installed, 1033, false
                    );

                result = marketplaceEntries
                         .Select(MakeNewExtension.FromGalleryEntry)
                         .OrderBy(e => e.Name)
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