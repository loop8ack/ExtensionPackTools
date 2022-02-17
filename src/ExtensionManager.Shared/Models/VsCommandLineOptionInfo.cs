using System;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ExtensionManager
{
    /// <summary>
    /// Encapsulates the information returned for a particular command-line option
    /// passed to <c>devenv.exe</c>.
    /// </summary>
    public class VsCommandLineOptionInfo : IVsCommandLineOptionInfo
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsAppCommandLine" />
        /// interface.
        /// </summary>
        private IVsAppCommandLine _vsAppCommandLine;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.VsCommandLineOptionInfo" /> and returns a
        /// reference to it.
        /// </summary>
        /// <param name="name">
        /// (Required.)  String containing the case-insensitive name of
        /// the command-line option to search for.
        /// </param>
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
        public VsCommandLineOptionInfo(string name,
            IVsAppCommandLine vsAppCommandLine)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Value cannot be null or whitespace.", nameof(name)
                );
            Name = name;
            _vsAppCommandLine = vsAppCommandLine ??
                                throw new ArgumentNullException(
                                    nameof(vsAppCommandLine)
                                );

            InitializeOption();
        }

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.VsCommandLineOptionInfo" /> and returns a
        /// reference to it.
        /// </summary>
        /// <remarks>
        /// This object's properties are set to their default values by this
        /// constructor.
        /// </remarks>
        public VsCommandLineOptionInfo()
        {
            Clear();
        }

        /// <summary>
        /// Gets a value that tells whether this object is uninitialized.
        /// </summary>
        public bool IsEmpty
            => string.IsNullOrWhiteSpace(Name) &&
               string.IsNullOrWhiteSpace(Value) && IsOnTheCommandLine == false &&
               _vsAppCommandLine == null;

        /// <summary>
        /// Gets or sets a string containing the name (case-insensitive) of the
        /// command-line option to search for.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the option was provided.
        /// </summary>
        public bool IsOnTheCommandLine { get; private set; }

        /// <summary>
        /// Gets the string value passed as an argument to the option, if any.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Resets the values of this object's properties to their defaults.
        /// </summary>
        public void Clear()
        {
            _vsAppCommandLine = null;
            Name = Value = string.Empty;
            IsOnTheCommandLine = false;
        }

        /// <summary>
        /// Attempts to gather the info for a particular command-line option.
        /// </summary>
        private void InitializeOption()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                if (string.IsNullOrWhiteSpace(Name)) return;
                if (_vsAppCommandLine == null) return;

                if (!ErrorHandler.Succeeded(
                        _vsAppCommandLine.GetOption(
                            Name, out var present, out var optionValue
                        )
                    )) return;
                Value = optionValue;
                IsOnTheCommandLine = present != 0;
            }
            catch (Exception ex)
            {
                // dump all the exception info to the log
                Debug.WriteLine(ex);

                Clear(); // reset all property values
            }
        }
    }
}