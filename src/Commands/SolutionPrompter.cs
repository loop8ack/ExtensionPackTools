using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public class SolutionPrompter
    {
        private readonly ExtensionService _extService;

        public SolutionPrompter(ExtensionService extService)
        {
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
                string msg = "This solution asks that you install the following extensions.";
                Importer.ImportWindow.Open(manifest.Extensions, Importer.Purpose.Install, msg);
            }
        }
    }
}
