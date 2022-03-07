using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.Setup.Configuration;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to obtain values.
    /// </summary>
    public static class Get
    {
        /// <summary>
        /// Gets a <see cref="T:System.String" /> that contains a default pathname of the
        /// temporary folder to use for downloads.
        /// </summary>
        /// <returns></returns>
        public static readonly string DefaultTempFolderPath = Path.Combine(
            Path.GetTempPath(), nameof(ExtensionManager)
        );

        /// <summary>
        /// Based on the value of the <paramref name="installSystemWide" /> flag, gets the
        /// switch to provide to <c>VSIXInstaller.exe</c> (or not, such as the case may
        /// be).
        /// </summary>
        /// <param name="installSystemWide">
        /// A <see cref="T:System.Boolean" /> whose value
        /// is set to <see langword="true" /> to instruct <c>VSIXInstaller.exe</c> to
        /// install extensions for all users, or <see langword="false" /> otherwise.
        /// </param>
        /// <returns>
        /// String containing the argument to be passed to
        /// <c>VSIXInstaller.exe</c> to enable installing an extension for all users, or
        /// blank if not specified.
        /// </returns>
        public static string AdminSwitch(bool installSystemWide)
        {
            return installSystemWide ? "/admin" : string.Empty;
        }

        /// <summary>
        /// Prompts the interactive user, with a Save As dialog box, to determine to which
        /// pathname the user would like the export saved.
        /// </summary>
        /// <param name="initialDirectory">
        /// (Optional.) The fully-qualified pathname of the
        /// folder where the Save As dialog box should be focused when it initially opens.
        /// <para />
        /// If this value is blank, then the dialog box opens focused on the <c>This PC</c>
        /// area of Windows instead.
        /// </param>
        /// <returns>
        /// String containing the fully-qualified pathname that the user chose in
        /// the dialog box; or, blank if an error occurred or the user clicked the
        /// <strong>Cancel</strong> button.
        /// </returns>
        /// <remarks>
        /// If the return value of this method is blank, then callers should not
        /// proceed.
        /// </remarks>
        public static string ExportFilePath(string initialDirectory = "")
        {
            var result = string.Empty;

            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Title = "Save Extension List Export As";
                    sfd.RestoreDirectory =
                        true; // do not reset the current working dir

                    // Start the user out in "This PC" so they can then drill down from there,
                    // Unless a valid folder path is specified for the argument of the
                    // initialDirectory parameter.
                    sfd.InitialDirectory =
                        DetermineAppropriateInitialDirectory(initialDirectory);

                    sfd.DefaultExt = ".vsext";
                    sfd.FileName = "extensions";
                    sfd.Filter =
                        "Visual Studio Extension List File (*.vsext)|*.vsext|All Files (*.*)|*.*";
                    sfd.CheckFileExists = sfd.CheckPathExists = true;

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return result;

                    result = sfd.FileName;
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Prompts the interactive user, with an Open dialog box, to determine from which
        /// pathname the user would like the list of extensions to import read.
        /// </summary>
        /// <param name="initialDirectory">
        /// (Optional.) The fully-qualified pathname of the
        /// folder where the Save As dialog box should be focused when it initially opens.
        /// <para />
        /// If this value is blank, then the dialog box opens focused on the <c>This PC</c>
        /// area of Windows instead.
        /// </param>
        /// <returns>
        /// String containing the fully-qualified pathname that the user chose in
        /// the dialog box; or, blank if an error occurred or the user clicked the
        /// <strong>Cancel</strong> button.
        /// </returns>
        /// <remarks>
        /// If the return value of this method is blank, then callers should not
        /// proceed.
        /// </remarks>
        public static string ImportFilePath(string initialDirectory = "")
        {
            var result = string.Empty;

            try
            {
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Title = "Open Extension List for Import";
                    ofd.RestoreDirectory =
                        true; // do not reset the current working dir

                    // Start the user out in "This PC" so they can then drill down from there,
                    // unless a valid folder path is specified for the argument of the
                    // initialDirectory parameter.
                    ofd.InitialDirectory =
                        DetermineAppropriateInitialDirectory(initialDirectory);

                    ofd.DefaultExt = ".vsext";
                    ofd.FileName = "extensions";
                    ofd.Filter =
                        "Visual Studio Extension List File (*.vsext)|*.vsext|All Files (*.*)|*.*";
                    ofd.CheckFileExists = ofd.CheckPathExists = true;

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return result;

                    result = ofd.FileName;
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Attempts to list the file names of all the files in the folder having the
        /// specified <paramref name="pathname" /> having an extension of <c>.vsix</c>.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the folder to be searched.
        /// </param>
        /// <returns>
        /// Collection of strings, each one containing the filename of a
        /// <c>.vsix</c> file in the folder having the specified
        /// <paramref name="pathname" />.
        /// <para />
        /// If the specified folder does not contain any <c>.vsix</c> files, then the empty
        /// collection is returned.
        /// <para />
        /// If an I/O or operating system error occurs during
        /// the operation, the empty collection is also returned.
        /// </returns>
        public static IList<string> ListOfVSIXFilenamesInFolder(string pathname)
        {
            var result = new List<string>();

            try
            {
                if (!Directory.EnumerateFiles(pathname, "*.vsix")
                              .Any()) return result;

                result = Directory.EnumerateFiles(pathname, "*.vsix")
                                  .Select(Path.GetFileName)
                                  .ToList();
            }
            catch
            {
                result = new List<string>();
            }

            return result;
        }

        /// <summary>
        /// Gets a reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Setup.Configuration.ISetupConfiguration" />
        /// for the currently-running Visual Studio instance.
        /// </summary>
        /// <returns>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Setup.Configuration.ISetupConfiguration" />
        /// interface.
        /// </returns>
        private static ISetupConfiguration TheSetupConfiguration()
        {
            return new SetupConfiguration();
        }

        /// <summary>
        /// For the <c>VSIXInstaller.exe</c> <c>/instanceids</c> switch, gets the Instance
        /// ID of the currently-running Visual Studio process.
        /// </summary>
        /// <returns>
        /// String containing the value to use for the <c>/instanceids</c> switch,
        /// or blank if an error occurred in fetching the value.
        /// </returns>
        public static string VisualStudioInstanceId()
        {
            string result;

            try
            {
                result = TheSetupConfiguration()
                         .GetInstanceForCurrentProcess()
                         .GetInstanceId();
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Gets the path to <c>VSIXInstaller.exe</c>, assuming that it lies in the same
        /// folder as does the main module of the specified <paramref name="process" />.
        /// </summary>
        /// <param name="process">
        /// (Required.) Reference to an instance of
        /// <see cref="T:System.Diagnostics.Process" /> that represents the
        /// currently-running process.
        /// </param>
        /// <returns>
        /// If the <c>VSIXInstaller.exe</c> file cannot be located at the assumed
        /// path, then the empty string is returned.
        /// <para />
        /// Otherwise, the fully-qualified pathname of the <c>VSIXInstaller.exe</c> file is
        /// returned.
        /// </returns>
        public static string VSIXInstallerPath(Process process)
        {
            var result = string.Empty;

            if (process == null) return result;

            try
            {
                var mainModuleFileName = process.MainModule?.FileName;
                if (!File.Exists(mainModuleFileName)) return result;

                var mainModuleFolder =
                    Path.GetDirectoryName(mainModuleFileName);
                if (string.IsNullOrWhiteSpace(mainModuleFolder))
                    return result;

                result = Directory.Exists(mainModuleFolder)
                    ? Path.Combine(mainModuleFolder, "VSIXInstaller.exe")
                    : string.Empty;

                if (!File.Exists(result))
                    return string.Empty;
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

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
        private static string DetermineAppropriateInitialDirectory(
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