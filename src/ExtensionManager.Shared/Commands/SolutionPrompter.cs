using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public class SolutionPrompter
    {
        private readonly ExtensionService _extService;
        private readonly Package _package;

        public SolutionPrompter(Package package, ExtensionService extService)
        {
            _package = package;
            _extService = extService;
        }

        public void Check(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!File.Exists(fileName))
            {
                return;
            }

            var manifest = Manifest.FromFile(fileName);
            IEnumerable<Extension> installed = _extService.GetInstalledExtensions();
            manifest.MarkSelected(installed);

            if (manifest.Extensions.Any(e => e.Selected))
            {
                var msg = "This solution asks that you install the following extensions.";
                var dialog = Importer.ImportWindow.Open(manifest.Extensions, Importer.Purpose.InstallForSolution, msg);

                if (dialog.DialogResult == true && dialog.SelectedExtension.Any())
                {
                    new ExtensionInstaller(_package)
                        .Install(dialog.SelectedExtension, dialog.InstallSystemWide);
                }
            }
        }
    }
}
