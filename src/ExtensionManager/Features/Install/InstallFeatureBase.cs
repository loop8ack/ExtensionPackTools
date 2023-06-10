using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Install;

public abstract class InstallFeatureBase : IFeature
{
    protected IExtensionInstaller Installer { get; }
    protected IDialogService DialogService { get; }
    protected IManifestService ManifestService { get; }

    protected InstallFeatureBase(IExtensionInstaller installer, IDialogService dialogService, IManifestService manifestService)
    {
        Installer = installer ?? throw new ArgumentNullException(nameof(installer));
        DialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        ManifestService = manifestService ?? throw new ArgumentNullException(nameof(manifestService));
    }

    public async Task ExecuteAsync()
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        var installedExtensions = await VSFacade.Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);
        var manifest = await ManifestService.ReadAsync(filePath).ConfigureAwait(false);
        var result = await ShowInstallDialogAsync(manifest, installedExtensions).ConfigureAwait(false);

        if (result is null)
            return;

        if (result.Extensions.Count > 0)
            await Installer.InstallAsync(result.Extensions, result.SystemWide).ConfigureAwait(false);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task<InstallExtensionsDialogResult?> ShowInstallDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions);
}
