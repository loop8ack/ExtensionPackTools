using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager.Shared.Helpers
{
    /// <summary>
    /// Exposes static extension methods for objects that implement the
    /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IInstalledExtension" />
    /// interface.
    /// </summary>
    public static class InstalledExtensionHelper
    {
        /// <summary>
        /// Safely obtains the identifier of the specified
        /// <paramref name="installedExtension" />.
        /// </summary>
        /// <param name="installedExtension">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IInstalledExtension" />
        /// interface.
        /// </param>
        /// <returns>
        /// String containing the identifier of the specified
        /// <paramref name="installedExtension" />.
        /// <para />
        /// If the method is passed a <see langword="null" /> value, or if the header
        /// information is missing, then this method returns the empty string.
        /// </returns>
        public static string GetIdentiifer(
            this IInstalledExtension installedExtension)
        {
            return installedExtension?.Header?.Identifier;
        }
    }
}