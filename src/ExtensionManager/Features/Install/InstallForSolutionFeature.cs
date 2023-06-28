using System.Collections.Generic;
using System.Threading.Tasks;

using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features.Install;

public sealed class InstallForSolutionFeature : InstallFeatureBase
{
    private readonly IVSSolutions _solutions;

    public InstallForSolutionFeature(Args args, IVSSolutions solutions)
        : base(args)
    {
        _solutions = solutions;
    }

    protected override async Task<string?> GetFilePathAsync()
        => await _solutions.GetCurrentSolutionExtensionsManifestFilePathAsync(MessageBox);

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowInstallForSolutionDialogAsync(worker, manifest, installedExtensions);
}
