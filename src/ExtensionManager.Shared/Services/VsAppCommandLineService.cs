using System;
using System.Diagnostics;
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
        private readonly IVsAppCommandLine _vsAppCommandLine;

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

        /// <summary>
        /// Gets the data about a particular Visual Studio command-line option having the
        /// specified <paramref name="name" /> (case-insensitive).
        /// </summary>
        /// <param name="name">
        /// (Required.) String containing the (case-insensitive) name of
        /// the option that you want to obtain information for.
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IVsCommandLineOptionInfo" /> interface whose
        /// properties are initialized with the data about the particular option you
        /// requested, or with default values if the option was not found on the command
        /// line of the currently-running instance of Visual Studio.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Thrown if the required parameter,
        /// <paramref name="name" />, is passed a blank or <see langword="null" /> string
        /// for a value.
        /// </exception>
        public IVsCommandLineOptionInfo GetOption(string name)
        {
            IVsCommandLineOptionInfo result = new VsCommandLineOptionInfo();

            if (string.IsNullOrWhiteSpace(name)) return result;

            try
            {
                /*
                 * The VsCommandLineOptionInfo class knows how to initialize
                 * itself, so long as you pass it a valid _vsAppCommandLine
                 * object reference.
                 */

                result = new VsCommandLineOptionInfo(name, _vsAppCommandLine);
            }
            catch (Exception ex)
            {
                // dump all the exception info to the log
                Debug.WriteLine(ex);

                result = new VsCommandLineOptionInfo();
            }

            return result;
        }
    }
}