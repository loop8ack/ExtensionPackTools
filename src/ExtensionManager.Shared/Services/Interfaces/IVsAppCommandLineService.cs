using Microsoft.VisualStudio.Shell.Interop;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a service that gets the
    /// command-line options passed to the Visual Studio Shell in which this extension
    /// is installed.
    /// </summary>
    public interface IVsAppCommandLineService
    {
        /// <summary>
        /// Gets a reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface from the global service provider for the Visual Studio Shell.
        /// </summary>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface, or <see langword="null" /> if an error occurred while attempting to
        /// obtain the reference.
        /// </returns>
        IVsAppCommandLine GetShellCommandLine();
    }
}