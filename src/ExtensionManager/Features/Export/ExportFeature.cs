using System.Collections.Generic;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Export;

public sealed class ExportFeature : ExportFeatureBase
{
    public ExportFeature(Args args)
        : base(args)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowSaveVsextFileDialogAsync();

    protected override async Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions)
        => await DialogService.ShowExportDialogAsync(worker, manifest, installedExtensions);

    protected override async Task OnManifestWrittenAsync(string filePath)
        => await Documents.OpenAsync(filePath);
}
