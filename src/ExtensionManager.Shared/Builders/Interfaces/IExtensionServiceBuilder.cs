
using ExtensionManager.Core.Services.Interfaces;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of an object that builds
    /// new instances of objects that implement the Reference to an instance of an
    /// object that implements the <see cref="T:ExtensionManager.IExtensionService" />
    /// interface.
    /// </summary>
    public interface IExtensionServiceBuilder
    {
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
        IExtensionService AndVsExtensionRepository(
            IVsExtensionRepository repository);

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
        IExtensionServiceBuilder HavingVsExtensionManager(
            IVsExtensionManager manager);
    }
}