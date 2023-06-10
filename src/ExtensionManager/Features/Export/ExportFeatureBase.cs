using System;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.VisualStudio;

namespace ExtensionManager.Features.Export;

public abstract class ExportFeatureBase : IFeature
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
        var accept = await DialogService.ShowExportDialogAsync(manifest, installedExtensions).ConfigureAwait(false);

        if (!accept)
            return;

        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        await ManifestService.WriteAsync(filePath, manifest).ConfigureAwait(false);
        await OnManifestWrittenAsync(filePath).ConfigureAwait(false);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task OnManifestWrittenAsync(string filePath);
}
