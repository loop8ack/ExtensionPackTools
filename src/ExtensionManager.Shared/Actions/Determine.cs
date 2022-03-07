using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods for determining values given constraints/inputs/other values.
    /// </summary>
    public static class Determine
    {
        /// <summary>
        /// Helper method to run an algorithm to set the initial directory for Open and
        /// Save As dialog boxes to 'This PC' unless the caller has specified otherwise.
        /// </summary>
        /// <param name="initialDirectory">
        /// (Optional.) String containing the
        /// fully-qualified pathname of the folder to use for the initial directory.
        /// </param>
        /// <returns>
        /// If <paramref name="initialDirectory" /> is blank, or if the pathname
        /// provided does not exist, then 'This PC' is returned.
        /// <para />
        /// Otherwise, the provided pathname is returned.
        /// </returns>
        public static string AppropriateInitialDirectoryForImportAndExport(
            string initialDirectory = "")
        {
            return !string.IsNullOrWhiteSpace(initialDirectory) &&
                   Directory.Exists(initialDirectory)
                ? initialDirectory
                : Environment.GetFolderPath(
                    Environment.SpecialFolder.MyComputer
                );
        }
    }
}
