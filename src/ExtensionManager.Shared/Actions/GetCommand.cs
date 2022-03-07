using System;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to obtain a reference to the desired command object.
    /// </summary>
    public static class GetCommand
    {
        /// <summary>
        /// Given a specified <paramref name="purpose" /> value, obtains a reference to the
        /// instance of the object implementing the
        /// <see cref="T:ExtensionManager.IExtensionManagerCommand" /> interface for the
        /// command object that corresponds to it.
        /// </summary>
        /// <param name="purpose">
        /// (Required.) A <see cref="T:ExtensionManager.Purpose" />
        /// value that specifies what command you want (for what purpose).
        /// </param>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionManagerCommand" /> interface that plays
        /// the role of the command object that is designed for the specified
        /// <paramref name="purpose" />.
        /// </returns>
        public static IExtensionManagerCommand For(Purpose purpose)
        {
            IExtensionManagerCommand command;

            switch (purpose)
            {
                case Purpose.Import:
                    command = ImportCommand.Instance;
                    break;

                case Purpose.Export:
                    command = ExportCommand.Instance;
                    break;

                case Purpose.InstallForSolution:
                    command = ExportSolutionCommand.Instance;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(purpose), purpose,
                        $"The '{purpose}' command is not supported."
                    );
            }

            return command;
        }
    }
}