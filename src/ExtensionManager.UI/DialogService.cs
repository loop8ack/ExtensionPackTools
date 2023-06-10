using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ExtensionManager.Manifest;
using ExtensionManager.UI.ViewModels;
using ExtensionManager.UI.Views;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

using Microsoft.Win32;

using WpfApplication = System.Windows.Application;

namespace ExtensionManager.UI;

internal sealed class DialogService : IDialogService
{
    public Task<string?> ShowSaveVsextFileDialogAsync() => ShowVsextFileDialogAsync<SaveFileDialog>();
    public Task<string?> ShowOpenVsextFileDialogAsync() => ShowVsextFileDialogAsync<OpenFileDialog>();
    private Task<string?> ShowVsextFileDialogAsync<TFileDialog>()
        where TFileDialog : FileDialog, new()
    {
        if (VSFacade.Threads.CheckUIThreadAccess())
            return Task.FromResult(OnUIThread());

        return VSFacade.Threads.RunOnUIThreadAsync(OnUIThread);

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

    public Task<bool> ShowExportDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowExportDialogAsync(manifest, installedExtensions, forSolution: false);
    public Task<bool> ShowExportForSolutionDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowExportDialogAsync(manifest, installedExtensions, forSolution: true);
    private async Task<bool> ShowExportDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions, bool forSolution)
    {
        var vm = new InstallExportDialogViewModel(forSolution ? InstallExportDialogType.ExportSolution : InstallExportDialogType.Export);

        foreach (var ext in installedExtensions)
            vm.Extensions.Add(new(ext));

        var accept = await ShowInstallExportDialogAsync(vm).ConfigureAwait(false);

        if (accept)
        {
            manifest.Extensions.Clear();

            foreach (var ext in vm.SelectedExtensions)
                manifest.Extensions.Add(ext.Model);
        }

        return accept;
    }

    public Task<InstallExtensionsDialogResult?> ShowInstallDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowInstallForSolutionDialogAsync(manifest, installedExtensions, forSolution: false);
    public Task<InstallExtensionsDialogResult?> ShowInstallForSolutionDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions)
        => ShowInstallForSolutionDialogAsync(manifest, installedExtensions, forSolution: true);
    private async Task<InstallExtensionsDialogResult?> ShowInstallForSolutionDialogAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> installedExtensions, bool forSolution)
    {
        var vm = new InstallExportDialogViewModel(forSolution ? InstallExportDialogType.InstallSolution : InstallExportDialogType.Install);

        foreach (var ext in manifest.Extensions)
        {
            var isInstalled = installedExtensions.Contains(ext, ExtensionEqualityComparer.Instance);

            vm.Extensions.Add(new(ext)
            {
                CanBeSelected = !isInstalled,
                Group = isInstalled ? "Already installed" : "Extensions",
            });
        }

        var accept = await ShowInstallExportDialogAsync(vm).ConfigureAwait(false);

        if (accept)
            return new(vm.SystemWide, vm.SelectedExtensions.Select(x => x.Model).ToArray());

        return null;
    }

    private Task<bool> ShowInstallExportDialogAsync(object viewModel)
    {
        if (VSFacade.Threads.CheckUIThreadAccess())
            return Task.FromResult(OnUIThread(viewModel));

        return VSFacade.Threads.RunOnUIThreadAsync(() => OnUIThread(viewModel));

        static bool OnUIThread(object viewModel)
        {
            var window = new InstallExportDialogWindow
            {
                Owner = WpfApplication.Current.MainWindow,
                DataContext = viewModel
            };

            var result = window.ShowDialog();

            return result == true;
        }
    }
}

file sealed class ExtensionEqualityComparer : IEqualityComparer<IVSExtension>
{
    public static ExtensionEqualityComparer Instance { get; } = new();

    public bool Equals(IVSExtension x, IVSExtension y)
        => x?.Id == y?.Id;

    public int GetHashCode(IVSExtension obj)
        => obj?.Id?.GetHashCode() ?? 0;
}
