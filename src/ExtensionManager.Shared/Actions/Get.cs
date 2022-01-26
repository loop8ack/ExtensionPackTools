using System;
using System.IO;
using System.Windows.Forms;

namespace ExtensionManager.Shared.Actions
{
    public static class Get
    {
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
                        DetermineAppropriateInitialDirectory(
                            initialDirectory
                        );

                    sfd.DefaultExt = ".vsext";
                    sfd.FileName = "extensions";
                    sfd.Filter =
                        "Visual Studio Extension List Export File (*.vsext)|*.vsext";
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