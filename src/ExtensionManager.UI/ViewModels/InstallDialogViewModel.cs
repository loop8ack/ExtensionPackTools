using System.Diagnostics;

using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI.ViewModels;

internal class InstallDialogViewModel : InstallExportDialogViewModel<InstallStep>
{
    private readonly IInstallWorker _worker;
    private readonly IManifest _manifest;

    public InstallDialogViewModel(IInstallWorker worker, IManifest manifest, bool forSolution)
        : base(forSolution ? InstallExportDialogType.InstallSolution : InstallExportDialogType.Install)
    {
        _worker = worker;
        _manifest = manifest;
    }

    public void AddExtension(IVSExtension extension, bool isInstalled)
    {
        Extensions.Add(new(extension)
        {
            CanBeSelected = !isInstalled,
            Group = isInstalled ? "Already installed" : "Extensions",
        });
    }

    protected override async Task DoWorkAsync(IProgress<ProgressStep<InstallStep>> progress, CancellationToken cancellationToken)
    {
        var extensions = SelectedExtensions.Select(x => x.Model).ToArray();

        await _worker.InstallAsync(_manifest, extensions, SystemWide, progress, cancellationToken);
    }

    protected override string? GetStepMessage(InstallStep step)
    {
        return step switch
        {
            InstallStep.None => null,
            InstallStep.DownloadData => "Download extension data",
            InstallStep.DownloadVsix => "Download extension files",
            InstallStep.RunInstallation => "Start installation",
            _ => ReturnWithDebuggerBreak(),
        };

        static string? ReturnWithDebuggerBreak()
        {
            Debugger.Break();

            return null;
        }
    }
}
