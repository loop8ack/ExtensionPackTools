using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;

namespace ExtensionPackTools
{
    internal sealed class ExportSolutionCommand
    {
        private readonly Package _package;

        private ExportSolutionCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            var cmdId = new CommandID(PackageGuids.guidExportPackageCmdSet, PackageIds.ExportSolutionCmd);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        public static ExportSolutionCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService)
        {
            Instance = new ExportSolutionCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            var manager = ServiceProvider.GetService(typeof(SVsExtensionManager)) as IVsExtensionManager;
            var repository = ServiceProvider.GetService(typeof(SVsExtensionRepository)) as IVsExtensionRepository;
            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;

            string fileName = Path.ChangeExtension(dte.Solution.FileName, ".vsext");

            try
            {
                var extensions = ExtensionHelpers.GetInstalledExtensions(manager, repository).ToList(); 

                if (File.Exists(fileName))
                {
                    var manifest = Manifest.FromFile(fileName);
                    extensions = extensions.Union(manifest.Extensions).ToList();

                    foreach (Extension ext in extensions)
                    {
                        ext.Selected = manifest.Extensions.Contains(ext);
                    }
                }

                var dialog = Importer.ImportWindow.Open(extensions, Importer.Purpose.List);

                if (dialog.DialogResult == true)
                {
                    var manifest = new Manifest(dialog.SelectedExtension);
                    string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

                    File.WriteAllText(fileName, json);
                    VsShellUtilities.OpenDocument(ServiceProvider, fileName);
                }
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    ex.Message,
                    Vsix.Name,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }

        private bool TryGetFilePath(out string filePath)
        {
            filePath = null;

            using (var sfd = new SaveFileDialog())
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
