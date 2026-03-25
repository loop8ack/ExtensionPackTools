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
    {
        var existingManifestFile = await _solutions.GetCurrentSolutionExtensionsManifestFilePathAsync(MessageBox).ConfigureAwait(false);

        var existingManifest = existingManifestFile?.Length > 0
            ? await ManifestService.ReadAsync(existingManifestFile).ConfigureAwait(false)
            : null;

        await DialogService.ShowExportForSolutionDialogAsync(worker, manifest, installedExtensions, existingManifest?.Extensions ?? []);
    }

    protected override async Task OnManifestWrittenAsync(string filePath)
    {
        const string folderName = "Solution Items";

        var solution = await _solutions.GetCurrentOrThrowAsync();
        var solutionChildren = await solution.GetChildrenAsync();

        var folder = solutionChildren.SingleOrDefault(x => x.Name == folderName) as IVSSolutionFolder
            ?? await solution.AddSolutionFolderAsync(folderName);

        if (folder is null)
            throw new InvalidOperationException("Could not add solution folder");

        await folder.AddExistingFilesAsync(filePath);
        await Documents.OpenAsync(filePath);
    }
}
