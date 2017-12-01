using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ExtensionPackTools
{
    internal sealed class ExportCommand
    {
        private readonly Package _package;

        private ExportCommand(Package package, OleMenuCommandService commandService)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            var cmdId = new CommandID(PackageGuids.guidExportPackageCmdSet, PackageIds.ExportCmd);
            var cmd = new MenuCommand(Execute, cmdId);
            commandService.AddCommand(cmd);
        }

        public static ExportCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(AsyncPackage package, OleMenuCommandService commandService)
        {
            Instance = new ExportCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            if (!TryGetFilePath(out string filePath))
            {
                return;
            }

            var manager = ServiceProvider.GetService(typeof(SVsExtensionManager)) as IVsExtensionManager;
            var repository = ServiceProvider.GetService(typeof(SVsExtensionRepository)) as IVsExtensionRepository;

            try
            {
                var installed = manager.GetInstalledExtensions()
                                       .Where(i => !i.Header.SystemComponent && !i.IsPackComponent)
                                       .Select(i => i.Header.Identifier);

                var marketplaceEntries = repository.GetVSGalleryExtensions<GalleryEntry>(installed.ToList(), 1033, false);
                Manifest manifest = new Manifest(marketplaceEntries);
                string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

                File.WriteAllText(filePath, json);
                VsShellUtilities.OpenDocument(ServiceProvider, filePath);
            }
            catch (Exception)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    "Something went wrong",
                    Vsix.Name,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    Microsoft.VisualStudio.Shell.Interop.OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
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

                var result = sfd.ShowDialog();

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
