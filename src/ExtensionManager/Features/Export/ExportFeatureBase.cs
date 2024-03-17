using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

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

        public Args(IThisVsixInfo vsixInfo, IVSDocuments documents, IVSMessageBox messageBox, IVSExtensions extensions, IDialogService dialogService, IManifestService manifestService)
        {
            VsixInfo = vsixInfo;
            Documents = documents;
            MessageBox = messageBox;
            Extensions = extensions;
            DialogService = dialogService;
            ManifestService = manifestService;
        }
    }

    private readonly Args _args;

    protected IThisVsixInfo VsixInfo => _args.VsixInfo;
    protected IVSDocuments Documents => _args.Documents;
    protected IVSMessageBox MessageBox => _args.MessageBox;
    protected IVSExtensions Extensions => _args.Extensions;
    protected IDialogService DialogService => _args.DialogService;
    protected IManifestService ManifestService => _args.ManifestService;

    protected ExportFeatureBase(Args args)
    {
        _args = args;
    }

    public async Task ExecuteAsync()
    {
        var manifest = ManifestService.CreateNew();
        var installedExtensions = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);

        var installedExtensionsList = installedExtensions as List<IVSExtension>
            ?? installedExtensions.ToList();

        installedExtensionsList.RemoveAll(vsix => vsix.Id == VsixInfo.Id);

        await ShowExportDialogAsync(manifest, this, installedExtensions);
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
    protected abstract Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions);
    protected abstract Task OnManifestWrittenAsync(string filePath);
}
