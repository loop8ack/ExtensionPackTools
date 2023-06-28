using System.Collections.Generic;
using System.Threading.Tasks;

using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Install;

public sealed class InstallFeature : InstallFeatureBase
{
    public InstallFeature(IExtensionInstaller installer, IDialogService dialogService, IManifestService manifestService)
        : base(installer, dialogService, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowOpenVsextFileDialogAsync();

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowInstallDialogAsync(worker, manifest, installedExtensions);
}
