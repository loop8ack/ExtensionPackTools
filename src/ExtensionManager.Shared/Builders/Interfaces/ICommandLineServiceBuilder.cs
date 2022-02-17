using Microsoft.VisualStudio.Shell.Interop;

namespace ExtensionManager
{
    /// <summary>
    /// Builds instances of objects that implement the
    /// <see cref="T:ExtensionManager.ICommandLineService" /> interface having
    /// their dependencies properly initialized.
    /// </summary>
    public interface ICommandLineServiceBuilder
    {
        /// <summary>
        /// Manufactures a new instance of an object that implements the
        /// <see cref="T:ExtensionManager.ICommandLineService" /> interface and which
        /// depends on the specified <paramref name="commandLine" />.
        /// </summary>
        /// <param name="commandLine">
        /// (Required.) Reference to an instance of an object
        /// that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface.
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.ICommandLineService" /> interface.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="commandLine" />, is passed a <see langword="null" />
        /// value.
        /// </exception>
        ICommandLineService HavingCommandLine(
            IVsAppCommandLine commandLine);
    }
}