using System;
using ExtensionManager.Core.Services.Interfaces;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    public class MakeNew : IExtensionServiceBuilder
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </summary>
        private IVsExtensionManager _manager;

        /// <summary>
        /// Empty, static constructor to prohibit direct allocation of this class.
        /// </summary>
        static MakeNew() { }

        /// <summary>
        /// Empty, protected constructor to prohibit direct allocation of this class.
        /// </summary>
        protected MakeNew() { }

        /// <summary>
        /// Gets a reference to the one and only instance of the object that implements the
        /// <see cref="T:ExtensionManager.Shared.Builders.IExtensionServiceBuilder" />
        /// interface.
        /// </summary>
        public static IExtensionServiceBuilder ExtensionService { get; } =
            new MakeNew();

        /// <summary>
        /// Builds a new instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface given a reference
        /// to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository" />
        /// interface.
        /// </summary>
        /// <param name="repository">
        /// (Required.) Reference to an instance of an object that
        /// implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository" />
        /// interface.
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="repository" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        public IExtensionService AndVsExtensionRepository(
            IVsExtensionRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            return new ExtensionService(_manager, repository);
        }

        /// <summary>
        /// Starts the process of building a new <c>Extension Service</c> object by first
        /// initializing it with a reference to an instance of an object that implements
        /// the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </summary>
        /// <param name="manager">
        /// (Required.) Reference to an instance of an object that
        /// implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.Shared.Builders.IExtensionServiceBuilder" />
        /// interface.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="manager" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        public IExtensionServiceBuilder HavingVsExtensionManager(
            IVsExtensionManager manager)
        {
            _manager = manager ??
                       throw new ArgumentNullException(nameof(manager));
            return this;
        }
    }
}