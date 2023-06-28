using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

namespace ExtensionManager.Features.Install;

public abstract class InstallFeatureBase : IFeature, IInstallWorker
{
    public sealed class Args
    {
        public IVSExtensions Extensions { get; }
        public IVSMessageBox MessageBox { get; }
        public IDialogService DialogService { get; }
        public IExtensionInstaller Installer { get; }
        public IManifestService ManifestService { get; }

        public Args(IVSExtensions extensions, IVSMessageBox messageBox, IDialogService dialogService, IExtensionInstaller installer, IManifestService manifestService)
        {
            Extensions = extensions;
            MessageBox = messageBox;
            DialogService = dialogService;
            Installer = installer;
            ManifestService = manifestService;
        }
    }

    private readonly Args _args;

    protected IVSExtensions Extensions => _args.Extensions;
    protected IVSMessageBox MessageBox => _args.MessageBox;
    protected IDialogService DialogService => _args.DialogService;
    protected IExtensionInstaller Installer => _args.Installer;
    protected IManifestService ManifestService => _args.ManifestService;

    protected InstallFeatureBase(Args args)
    {
        _args = args;
    }

    public async Task ExecuteAsync()
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        var installedExtensions = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);
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
