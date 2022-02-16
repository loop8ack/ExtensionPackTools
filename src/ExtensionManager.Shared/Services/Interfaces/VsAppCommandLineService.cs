using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExtensionManager
{
    /// <summary>
    /// Object that accesses the Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine
    /// service in a robust and fault-tolerant manner.
    /// </summary>
    public class VsAppCommandLineService : IVsAppCommandLineService
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface.
        /// </summary>
        private IVsAppCommandLine _vsAppCommandLine;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.VsAppCommandLineService" /> and returns a
        /// reference to it.
        /// </summary>
        /// <param name="vsAppCommandLine">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="vsAppCommandLine" />, is passed a
        /// <see langword="null" /> value.
        /// </exception>
        public VsAppCommandLineService(IVsAppCommandLine vsAppCommandLine)
        {
            _vsAppCommandLine = vsAppCommandLine ??
                                throw new ArgumentNullException(
                                    nameof(vsAppCommandLine)
                                );
        }
    }
}