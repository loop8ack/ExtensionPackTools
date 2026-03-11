using System.Collections.ObjectModel;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features.Export;

public abstract class ExportFeatureBase : IFeature, IExportWorker
{
    public sealed class Args
    {
        public IThisVsixInfo VsixInfo { get; }
        public IVSDocuments Documents { get; }
        public IVSMessageBox MessageBox { get; }
        public IVSExtensions Extensions { get; }
        public IDialogService DialogService { get; }
        public IManifestService ManifestService { get; }
        public IVSSolutions Solutions { get; }

        public Args(IThisVsixInfo vsixInfo, IVSDocuments documents, IVSMessageBox messageBox, IVSExtensions extensions, IDialogService dialogService, IManifestService manifestService, IVSSolutions solutions)
        {
            VsixInfo = vsixInfo;
            Documents = documents;
            MessageBox = messageBox;
            Extensions = extensions;
            DialogService = dialogService;
            ManifestService = manifestService;
            Solutions = solutions;
        }
    }

    private readonly Args _args;

    protected IThisVsixInfo VsixInfo => _args.VsixInfo;
    protected IVSDocuments Documents => _args.Documents;
    protected IVSMessageBox MessageBox => _args.MessageBox;
    protected IVSExtensions Extensions => _args.Extensions;
    protected IDialogService DialogService => _args.DialogService;
    protected IManifestService ManifestService => _args.ManifestService;
    protected IVSSolutions Solutions => _args.Solutions;

    protected ExportFeatureBase(Args args)
    {
        _args = args;
    }

    public async Task ExecuteAsync()
    {
        IManifest manifest;

        var vsextFile = await Solutions.GetCurrentSolutionExtensionsManifestFilePathAsync(MessageBox).ConfigureAwait(false);

        if (vsextFile != null && !string.IsNullOrEmpty(vsextFile))
        {
            // Attempt to read manifest from the found .vsext file
            manifest = await ManifestService.ReadAsync(vsextFile).ConfigureAwait(false);
        }
        else
        {
            // No .vsext found: create new manifest
            manifest = ManifestService.CreateNew();
        }

        var installedExtensions = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);

        var installedExtensionsList = installedExtensions as List<IVSExtension>
            ?? installedExtensions.ToList();

        installedExtensionsList.RemoveAll(vsix => vsix.Id == VsixInfo.Id);

        var selectedExtensions = manifest.Extensions;

        await ShowExportDialogAsync(manifest, this, installedExtensions, new ReadOnlyCollection<IVSExtension>(selectedExtensions));
    }

    async Task IExportWorker.ExportAsync(IManifest manifest, IProgress<ProgressStep<ExportStep>> progress, CancellationToken cancellationToken)
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        progress.Report(null, ExportStep.SaveManifest);
        await ManifestService.WriteAsync(filePath, manifest, cancellationToken).ConfigureAwait(false);

        progress.Report(null, ExportStep.Finish);
        await OnManifestWrittenAsync(filePath);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions, IReadOnlyCollection<IVSExtension> selectedExtensions);
    protected abstract Task OnManifestWrittenAsync(string filePath);
}

