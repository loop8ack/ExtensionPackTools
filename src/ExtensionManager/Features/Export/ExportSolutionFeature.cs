using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features.Export;

public sealed class ExportSolutionFeature : ExportFeatureBase
{
    private readonly IVSSolutions _solutions;

    public ExportSolutionFeature(Args args, IVSSolutions solutions)
        : base(args)
    {
        _solutions = solutions;
    }

    protected override async Task<string?> GetFilePathAsync()
        => await _solutions.GetCurrentSolutionExtensionsManifestFilePathAsync(MessageBox);

    protected override async Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowExportForSolutionDialogAsync(worker, manifest, installedExtensions);

    protected override async Task OnManifestWrittenAsync(string filePath)
    {
        var solution = await _solutions.GetCurrentOrThrowAsync();
        var folder = await solution.AddSolutionFolderAsync("Solution Items");

        if (folder is null)
            throw new InvalidOperationException("Could not add solution folder");

        await folder.AddExistingFilesAsync(filePath);
        await Documents.OpenAsync(filePath);
    }
}
