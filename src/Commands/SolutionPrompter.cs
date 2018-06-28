using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    public class SolutionPrompter
    {
        private readonly IVsExtensionManager _manager;
        private readonly IVsExtensionRepository _repository;

        public SolutionPrompter(IVsExtensionManager manager, IVsExtensionRepository repository)
        {
            _manager = manager;
            _repository = repository;
        }
        public void Check(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            var manifest = Manifest.FromFile(fileName);
            IEnumerable<Extension> installed = Helpers.GetInstalledExtensions(_manager, _repository);
            manifest.MarkSelected(installed);

            if (manifest.Extensions.Any(e => e.Selected))
            {
                string msg = "This solution asks that you install the following extensions.";
                Importer.ImportWindow.Open(manifest.Extensions, Importer.Purpose.Install, msg);
            }
        }
    }
}
