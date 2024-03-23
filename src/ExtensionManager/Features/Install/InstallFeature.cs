using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;

namespace ExtensionManager.Features.Install;

public sealed class InstallFeature : InstallFeatureBase
{
    public InstallFeature(Args args)
        : base(args)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowOpenVsextFileDialogAsync();

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<VSExtensionToInstall> extensions)
        => await DialogService.ShowInstallDialogAsync(worker, manifest, extensions);
}
