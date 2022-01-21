using System.IO;
using System.Linq;
using ExtensionManager.Importer;
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

            if (!File.Exists(fileName)) return;

            var manifest = Manifest.FromFile(fileName);

            var installedExtensions = _extService.GetInstalledExtensions();
            manifest.MarkSelected(installedExtensions);

            if (!manifest.Extensions.Any(e => e.Selected))
                return;

            ImportWindow.Open(
                manifest.Extensions, Purpose.InstallForSolution,
                "This solution asks that you install the following extensions."
            );
        }
    }
}