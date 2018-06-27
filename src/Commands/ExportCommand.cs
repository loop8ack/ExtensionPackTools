using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using static ExtensionPackTools.Manifest;

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

        public static void Initialize(Package package, OleMenuCommandService commandService)
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
                                       .Where(i => !i.Header.SystemComponent && !i.IsPackComponent && i.State == EnabledState.Enabled)
                                       .Select(i => i.Header.Identifier)
                                       .ToList();

                // Filter the installed extensions to only be the ones that exist on the Marketplace
                IEnumerable<GalleryEntry> marketplaceEntries = repository.GetVSGalleryExtensions<GalleryEntry>(installed, 1033, false);
                IEnumerable<Extension> extensions = marketplaceEntries.Select(me => new Extension { ID = me.VsixID, Name = me.Name });

                var dialog = new Importer.ImportWindow(extensions, manager, Importer.Purpose.Export);
                dialog.ShowDialog();

                if (dialog.DialogResult == true)
                {
                    var manifest = new Manifest(marketplaceEntries.Where(me => dialog.SelectedExtensionIds.Contains(me.VsixID)));
                    string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

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
