using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Export;

public abstract class ExportFeatureBase : IFeature, IExportWorker
{
    protected IDialogService DialogService { get; }
    protected IManifestService ManifestService { get; }

    protected ExportFeatureBase(IDialogService dialogService, IManifestService manifestService)
    {
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        ManifestService = manifestService ?? throw new ArgumentNullException(nameof(manifestService));
    }

    public async Task ExecuteAsync()
    {
        var manifest = ManifestService.CreateNew();
        var installedExtensions = await VSFacade.Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);

        await ShowExportDialogAsync(manifest, this, installedExtensions);
    }

    async Task IExportWorker.ExportAsync(IManifest manifest, IProgress<ProgressStep<ExportStep>> progress, CancellationToken cancellationToken)
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        progress.Report(null, ExportStep.SaveManifest);
        await ManifestService.WriteAsync(filePath, manifest, cancellationToken).ConfigureAwait(false);

        progress.Report(null, ExportStep.Finish);
        await OnManifestWrittenAsync(filePath);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions);
    protected abstract Task OnManifestWrittenAsync(string filePath);
}
