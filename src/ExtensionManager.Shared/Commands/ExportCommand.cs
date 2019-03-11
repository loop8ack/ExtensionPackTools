using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;

namespace ExtensionManager
{
    internal sealed class ExportCommand
    {
        private readonly Package _package;
        private readonly ExtensionService _es;

        private ExportCommand(Package package, OleMenuCommandService commandService, ExtensionService es)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _es = es;

            var cmdId = new CommandID(PackageGuids.guidExportPackageCmdSet, PackageIds.ExportCmd);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        public static ExportCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider => _package;

        public static void Initialize(Package package, OleMenuCommandService commandService, ExtensionService es)
        {
            Instance = new ExportCommand(package, commandService, es);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                IEnumerable<Extension> extensions = _es.GetInstalledExtensions();

                var dialog = Importer.ImportWindow.Open(extensions, Importer.Purpose.List);

                if (dialog.DialogResult == true)
                {
                    if (!TrySaveFilePath(out string filePath))
                    {
                        return;
                    }

                    var manifest = new Manifest(dialog.SelectedExtension);
                    var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

                    File.WriteAllText(filePath, json);
                    VsShellUtilities.OpenDocument(ServiceProvider, filePath);
                }
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    ex.Message,
                    Vsix.Name,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }
    }
}
