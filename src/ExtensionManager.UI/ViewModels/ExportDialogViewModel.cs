using System.Diagnostics;

using ExtensionManager.Manifest;
using ExtensionManager.UI.Worker;

namespace ExtensionManager.UI.ViewModels;

internal class ExportDialogViewModel : InstallExportDialogViewModel<ExportStep>
{
    private readonly IExportWorker _worker;
    private readonly IManifest _manifest;

    public ExportDialogViewModel(IExportWorker worker, IManifest manifest, bool forSolution)
        : base(forSolution ? InstallExportDialogType.ExportSolution : InstallExportDialogType.Export)
    {
        _worker = worker;
        _manifest = manifest;
    }

    protected override async Task DoWorkAsync(IProgress<ProgressStep<ExportStep>> progress, CancellationToken cancellationToken)
    {
        _manifest.Extensions.Clear();

        foreach (var ext in SelectedExtensions)
            _manifest.Extensions.Add(ext.Model);

        await _worker.ExportAsync(_manifest, progress, cancellationToken);
    }

    protected override string? GetStepMessage(ExportStep step)
    {
        return step switch
        {
            ExportStep.None => null,
            ExportStep.SaveManifest => "Save manifest",
            ExportStep.Finish => "Finish export",
            _ => ReturnWithDebuggerBreak(),
        };

        static string? ReturnWithDebuggerBreak()
        {
            Debugger.Break();

            return null;
        }
    }
}
