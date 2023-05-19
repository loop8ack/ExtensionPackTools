using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager.V17_5
{
    public class VisualStudioService : IVisualStudioService
    {
        public static async Task<IVisualStudioService> CreateAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var repository = await package.GetServiceAsync(typeof(SVsExtensionRepository)) as IVsExtensionRepository;
            var manager = await package.GetServiceAsync(typeof(SVsExtensionManager)) as IVsExtensionManager;

            return new VisualStudioService(repository, manager);
        }

        private readonly IVsExtensionRepository _repository;
        private readonly IVsExtensionManager _manager;

        public VisualStudioService(IVsExtensionRepository repository, IVsExtensionManager manager)
        {
            _repository = repository;
            _manager = manager;
        }

        public IEnumerable<InstalledExtension> GetInstalledExtensions()
        {
            return _manager.GetInstalledExtensions()
                .Select(x => new InstalledExtension(x.Header.Identifier, x.Header.SystemComponent, x.IsPackComponent));
        }

        public IEnumerable<IGalleryEntry> GetGalleryEntries(List<string> extensionIds)
        {
            return _repository.GetVSGalleryExtensions<GalleryEntry>(extensionIds, 1033, false);
        }
    }
}
