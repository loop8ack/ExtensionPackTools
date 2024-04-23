using ExtensionManager.Manifest;
using ExtensionManager.UI.ViewModels;
using ExtensionManager.UI.Views;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.Threads;

using Microsoft.Win32;

using WpfApplication = System.Windows.Application;

namespace ExtensionManager.UI;

internal sealed class DialogService : IDialogService
{
    private readonly IVSThreads _threads;

    public DialogService(IVSThreads threads)
    {
        _threads = threads;
    }

    public Task<string?> ShowSaveVsextFileDialogAsync() => ShowVsextFileDialogAsync<SaveFileDialog>();
    public Task<string?> ShowOpenVsextFileDialogAsync() => ShowVsextFileDialogAsync<OpenFileDialog>();
    private Task<string?> ShowVsextFileDialogAsync<TFileDialog>()
        where TFileDialog : FileDialog, new()
    {
        if (_threads.CheckUIThreadAccess())
            return Task.FromResult(OnUIThread());

        return _threads.RunOnUIThreadAsync(OnUIThread);

        static string? OnUIThread()
        {
            TFileDialog dialog = new()
            {
                DefaultExt = ".vsext",
                FileName = "extensions",
                Filter = "VSEXT File|*.vsext"
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileName;

            return null;
        }
    }

    public Task ShowExportDialogAsync(IExportWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowExportDialogAsync(worker, manifest, installedExtensions, forSolution: false);
    public Task ShowExportForSolutionDialogAsync(IExportWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowExportDialogAsync(worker, manifest, installedExtensions, forSolution: true);
    private async Task ShowExportDialogAsync(IExportWorker worker, IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions, bool forSolution)
    {
        var vm = new ExportDialogViewModel(worker, manifest, forSolution);

        foreach (var ext in installedExtensions)
            vm.Extensions.Add(new(ext));

        await ShowInstallExportDialogAsync(vm);
    }

    public Task ShowInstallDialogAsync(IInstallWorker worker, IManifest manifest, IReadOnlyCollection<VSExtensionToInstall> extensions)
        => ShowInstallForSolutionDialogAsync(worker, manifest, extensions, forSolution: false);
    public Task ShowInstallForSolutionDialogAsync(IInstallWorker worker, IManifest manifest, IReadOnlyCollection<VSExtensionToInstall> extensions)
        => ShowInstallForSolutionDialogAsync(worker, manifest, extensions, forSolution: true);
    private async Task ShowInstallForSolutionDialogAsync(IInstallWorker worker, IManifest manifest, IReadOnlyCollection<VSExtensionToInstall> extensions, bool forSolution)
    {
        var vm = new InstallDialogViewModel(worker, manifest, forSolution);

        foreach (var (extension, status) in extensions)
        {
            switch (status)
            {
                case VSExtensionStatus.Installed:
                    vm.AddExtension(extension, canBeSelected: false, group: "Already installed");
                    break;

                case VSExtensionStatus.NotInstalled:
                    vm.AddExtension(extension, canBeSelected: true, group: "Extensions");
                    break;

                case VSExtensionStatus.NotSupported:
                    vm.AddExtension(extension, canBeSelected: false, group: "Not supported");
                    break;

                default:
                    throw new InvalidOperationException($"Unknown status {status} for extension {extension.Id}");
            }
        }

        await ShowInstallExportDialogAsync(vm);
    }

    private Task ShowInstallExportDialogAsync(object viewModel)
    {
        if (_threads.CheckUIThreadAccess())
        {
            OnUIThread(viewModel);

            return Task.CompletedTask;
        }

        return _threads.RunOnUIThreadAsync(() => OnUIThread(viewModel));

        static void OnUIThread(object viewModel)
        {
            var window = new InstallExportDialogWindow
            {
                Owner = WpfApplication.Current.MainWindow,
                DataContext = viewModel
            };

            try
            {
                window.ShowDialog();
            }
            catch
            {
                window.Close();

                throw;
            }
        }
    }
}
