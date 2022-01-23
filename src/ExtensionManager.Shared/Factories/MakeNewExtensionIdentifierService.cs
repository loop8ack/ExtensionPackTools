using System;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Creates new instances of objects that implement the
    /// <see cref="T:ExtensionManager.IExtensionIdentifierService" /> interface, and
    /// returns references to them.
    /// </summary>
    public static class MakeNewExtensionIdentifierService
    {
        /// <summary>
        /// Creates a new instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionIdentifierService" /> interface and
        /// returns a reference to it.
        /// </summary>
        /// <param name="manager">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager" />
        /// interface.
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionIdentifierService" /> interface.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="manager" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        public static IExtensionIdentifierService ForVsExtensionManager(
            IVsExtensionManager manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            return new ExtensionIdentifierService(manager);
        }
    }
}