using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ExtensionManager
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class VsPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            bool isSolutionLoaded = await IsSolutionLoadedAsync(solService);

            if (isSolutionLoaded)
            {
                JoinableTaskFactory.RunAsync(() =>
                {
                    return HandleOpenSolutionAsync(solService, cancellationToken);
                }).FileAndForget($"{nameof(ExtensionManager)}/{nameof(HandleOpenSolutionAsync)}"); ;
            }

            // Listen for subsequent solution events
            SolutionEvents.OnAfterOpenSolution += (s, e) => HandleOpenSolutionAsync(solService, cancellationToken).ConfigureAwait(false);

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(this, commandService);
                ExportSolutionCommand.Initialize(this, commandService);
                await ImportCommand.InitializeAsync(this, commandService);
            }
        }

        private async Task<bool> IsSolutionLoadedAsync(IVsSolution solService)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private async Task HandleOpenSolutionAsync(IVsSolution solService, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_SolutionFileName, out object value));

            if (value is string solFileName && !string.IsNullOrEmpty(solFileName))
            {
                string fileName = Path.ChangeExtension(solFileName, ".vsext");
                var manager = await GetServiceAsync(typeof(SVsExtensionManager)) as IVsExtensionManager;
                var repository = await GetServiceAsync(typeof(SVsExtensionRepository)) as IVsExtensionRepository;

                var prompter = new SolutionPrompter(manager, repository);
                prompter.Check(fileName);
            }
        }
    }
}
