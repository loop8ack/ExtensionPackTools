namespace ExtensionManager
{
    /// <summary>
    /// Values that designate the particular operation that the user desires to perform
    /// with the Importer.
    /// </summary>
    public enum Purpose
    {
        /// <summary>
        /// Prompt the user to install extensions to Visual Studio because a particular
        /// Solution (<c>.sln</c>) needs them.
        /// </summary>
        InstallForSolution,

        /// <summary>
        /// Importing extensions into Visual Studio from a <c>.vsext</c> JSON-formatted
        /// file that describes which extensions should be installed.
        /// </summary>
        Import,

        /// <summary>
        /// Exporting a list of the currently-installed Visual Studio extensions (only
        /// those that have been obtained from the Visual Studio Marketplace) and output
        /// that list to a JSON-formatted file with a <c>.vsext</c> extension.
        /// </summary>
        Export
    }
}