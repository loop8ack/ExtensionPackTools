using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI;

public interface IDialogService
{
    Task<string?> ShowSaveVsextFileDialogAsync();
    Task<string?> ShowOpenVsextFileDialogAsync();

    Task<bool> ShowExportDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task<bool> ShowExportForSolutionDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task<InstallExtensionsDialogResult?> ShowInstallDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
    Task<InstallExtensionsDialogResult?> ShowInstallForSolutionDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
}
