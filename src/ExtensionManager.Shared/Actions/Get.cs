using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

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
                    // Unless a valid folder path is specified for the argument of the
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