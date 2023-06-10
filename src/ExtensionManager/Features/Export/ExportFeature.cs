using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.VisualStudio;

namespace ExtensionManager.Features.Export;

public sealed class ExportFeature : ExportFeatureBase
{
    public ExportFeature(IDialogService dialogService, IManifestService manifestService)
        : base(dialogService, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowSaveVsextFileDialogAsync();

    protected override async Task OnManifestWrittenAsync(string filePath)
        => await VSFacade.Documents.OpenAsync(filePath);
}
