using System;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features.Export;

public sealed class ExportSolutionFeature : ExportFeatureBase
{
    public ExportSolutionFeature(IDialogService dialogService, IManifestService manifestService)
        : base(dialogService, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await VSFacade.Solutions.GetCurrentSolutionExtensionsManifestFilePathAsync();

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
