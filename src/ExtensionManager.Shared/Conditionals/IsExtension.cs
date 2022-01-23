using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to make decisions upon extensions.
    /// </summary>
    public static class IsExtension
    {
        /// <summary>
        /// Determines whether the specified <paramref name="installedExtension" /> is a
        /// user-installed extension.
        /// </summary>
        /// <param name="installedExtension">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.ExtensionManager.IInstalledExtension" />
        /// interface.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified
        /// <paramref name="installedExtension" /> was installed by the user;
        /// <see langword="false" /> otherwise.
        /// </returns>
        public static bool InstalledByTheUser(
            IInstalledExtension installedExtension)
        {
            if (installedExtension?.Header == null) return false;
            if (installedExtension.Header.SystemComponent) return false;
            return !installedExtension.IsPackComponent;
        }
    }
}