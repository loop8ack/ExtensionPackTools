namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of an object that
    /// encapsulates data about an option passed on the command line of the Visual
    /// Studio Shell.
    /// </summary>
    public interface IVsCommandLineOptionInfo
    {
        /// <summary>
        /// Gets or sets a string containing the name (case-insensitive) of the
        /// command-line option to search for.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the option was provided.
        /// </summary>
        bool IsProvided { get; }

        /// <summary>
        /// Gets the string value passed as an argument to the option, if any.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Gets a value that tells whether this object is uninitialized.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Resets the values of this object's properties to their defaults.
        /// </summary>
        void Clear();
    }
}