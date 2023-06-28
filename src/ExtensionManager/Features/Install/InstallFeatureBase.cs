using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Features.Install;

public abstract class InstallFeatureBase : IFeature, IInstallWorker
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
        
        await ShowInstallDialogAsync(manifest, this, installedExtensions);
    }

    async Task IInstallWorker.InstallAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> extensions, bool systemWide, IProgress<ProgressStep<InstallStep>> progress, CancellationToken cancellationToken)
    {
        if (extensions.Count > 0)
            await Installer.InstallAsync(extensions, systemWide, progress, cancellationToken);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions);
}
