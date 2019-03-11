using System.Windows.Forms;

namespace ExtensionManager
{
    public static class FilePathHelpers
    {
        public static bool TrySaveFilePath(out string filePath)
        {
            using (var sfd = new SaveFileDialog())
            {
                return TryGetFilePath(out filePath, sfd);
            }
        }

        public static bool TryOpenFilePath(out string filePath)
        {
            using (var ofd = new OpenFileDialog())
            {
                return TryGetFilePath(out filePath, ofd);
            }
        }

        private static bool TryGetFilePath(out string filePath, FileDialog dialog)
        {
            filePath = null;

            dialog.DefaultExt = ".vsext";
            dialog.FileName = "extensions";
            dialog.Filter = "VSEXT File|*.vsext";

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                filePath = dialog.FileName;
                return true;
            }

            return false;
        }
    }
}
