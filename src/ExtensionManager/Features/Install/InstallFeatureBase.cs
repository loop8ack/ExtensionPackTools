using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.Utils;
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

        if (!File.Exists(filePath))
            return;

        var manifest = await ManifestService.ReadAsync(filePath).ConfigureAwait(false);
        var extensionsToInstall = await CreateExtensionsToInstallListAsync(manifest.Extensions).ConfigureAwait(false);

        await ShowInstallDialogAsync(manifest, this, extensionsToInstall.ToList());
    }

    protected virtual async Task<IEnumerable<VSExtensionToInstall>> CreateExtensionsToInstallListAsync(IEnumerable<IVSExtension> toInstall)
    {
        var installed = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);
        var gallery = await Extensions.GetGalleryExtensionsAsync(toInstall.Select(x => x.Id)).ConfigureAwait(false);

        var statuses = new Dictionary<string, VSExtensionStatus>();

        foreach (var extension in toInstall)
            statuses[extension.Id] = VSExtensionStatus.NotSupported;

        foreach (var extension in gallery)
            statuses[extension.Id] = VSExtensionStatus.NotInstalled;

        foreach (var extension in installed.Intersect(toInstall, ExtensionEqualityComparer.Instance))
            statuses[extension.Id] = VSExtensionStatus.Installed;

        return toInstall
            .Distinct(ExtensionEqualityComparer.Instance)
            .Select(x => new VSExtensionToInstall(x, statuses[x.Id]));
    }

    async Task IInstallWorker.InstallAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> extensions, bool systemWide, IProgress<ProgressStep<InstallStep>> progress, CancellationToken cancellationToken)
    {
        if (extensions.Count > 0)
            await Installer.InstallAsync(extensions, systemWide, progress, cancellationToken);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<VSExtensionToInstall> extensions);
}
