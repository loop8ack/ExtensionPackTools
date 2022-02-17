namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a service that gets the
    /// command-line options passed to the Visual Studio Shell in which this extension
    /// is installed.
    /// </summary>
    public interface ICommandLineService
    {
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
        IVsCommandLineOptionInfo GetOption(string name);
    }
}