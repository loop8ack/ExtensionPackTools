using ExtensionManager.Manifest;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI.Worker;

public interface IInstallWorker
{
    Task InstallAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> extensions, bool systemWide, IProgress<ProgressStep<InstallStep>> progress, CancellationToken cancellationToken);
}
