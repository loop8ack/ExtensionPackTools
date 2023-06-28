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

public sealed class ExportSolutionFeature : ExportFeatureBase
{
    public ExportSolutionFeature(IDialogService dialogService, IManifestService manifestService)
        : base(dialogService, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await VSFacade.Solutions.GetCurrentSolutionExtensionsManifestFilePathAsync();

    protected override async Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowExportForSolutionDialogAsync(worker, manifest, installedExtensions);

    protected override async Task OnManifestWrittenAsync(string filePath)
    {
        var solution = await VSFacade.Solutions.GetCurrentOrThrowAsync();
        var folder = await solution.AddSolutionFolderAsync("Solution Items");

        if (folder is null)
            throw new InvalidOperationException("Could not add solution folder");

        await folder.AddExistingFilesAsync(filePath);
        await VSFacade.Documents.OpenAsync(filePath);
    }
}
