using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Forms;

using ExtensionManager.Importer;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    internal sealed class ImportCommand
    {
        private readonly Package _package;
        private readonly ExtensionService _es;

        private ImportCommand(Package package, OleMenuCommandService commandService, ExtensionService es)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _es = es;

            var cmdId = new CommandID(PackageGuids.guidExportPackageCmdSet, PackageIds.ImportCmd);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        public static ImportCommand Instance { get; private set; }

        public static void Initialize(AsyncPackage package, OleMenuCommandService commandService, ExtensionService es)
        {
            Instance = new ImportCommand(package, commandService, es);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!TryGetFilePath(out var filePath))
            {
                return;
            }

            var manifest = Manifest.FromFile(filePath);
            manifest.MarkSelected(_es.GetInstalledExtensions());

            var dialog = ImportWindow.Open(manifest.Extensions, Purpose.Import);

            if (dialog.DialogResult == true && dialog.SelectedExtension.Any())
            {
                new ExtensionInstaller(_package)
                    .Install(dialog.SelectedExtension, dialog.InstallSystemWide);
            }
        }

        public static bool TryGetFilePath(out string filePath)
        {
            filePath = null;

            using (var sfd = new OpenFileDialog())
            {
                sfd.DefaultExt = ".vsext";
                sfd.FileName = "extensions";
                sfd.Filter = "VSEXT File|*.vsext";

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    filePath = sfd.FileName;
                    return true;
                }
            }

            return false;
        }
    }
}
