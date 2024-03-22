using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI;

public interface IDialogService
{
    Task<string?> ShowSaveVsextFileDialogAsync();
    Task<string?> ShowOpenVsextFileDialogAsync();

    Task ShowExportDialogAsync(IExportWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task ShowExportForSolutionDialogAsync(IExportWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task ShowInstallDialogAsync(IInstallWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task ShowInstallForSolutionDialogAsync(IInstallWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
}
