using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Install;

public sealed class InstallFeature : InstallFeatureBase
{
    public InstallFeature(Args args)
        : base(args)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowOpenVsextFileDialogAsync();

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowInstallDialogAsync(worker, manifest, installedExtensions);
}
