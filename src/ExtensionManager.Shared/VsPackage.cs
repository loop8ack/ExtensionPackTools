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
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class VsPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var manager = await GetServiceAsync(typeof(SVsExtensionManager)) as IVsExtensionManager;
            var repository = await GetServiceAsync(typeof(SVsExtensionRepository)) as IVsExtensionRepository;

            var extService = new ExtensionService(manager, repository);
            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            bool isSolutionLoaded = await IsSolutionLoadedAsync(solService);

            if (isSolutionLoaded)
            {
                JoinableTaskFactory.RunAsync(() =>
                {
                    return HandleOpenSolutionAsync(solService, extService, cancellationToken);
                }).FileAndForget($"{nameof(ExtensionManager)}/{nameof(HandleOpenSolutionAsync)}"); ;
            }

            // Listen for subsequent solution events
            SolutionEvents.OnAfterOpenSolution += (s, e) => HandleOpenSolutionAsync(solService, extService, cancellationToken).ConfigureAwait(false);

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(this, commandService, extService);
                ExportSolutionCommand.Initialize(this, commandService, extService);
                ImportCommand.Initialize(this, commandService, extService);
            }
        }

        private async Task<bool> IsSolutionLoadedAsync(IVsSolution solService)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out object value));

            return value is bool isSolOpen && isSolOpen;
        }

        private async Task HandleOpenSolutionAsync(IVsSolution solService, ExtensionService es, CancellationToken cancellationToken)
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

                await ThreadHelper.JoinableTaskFactory.StartOnIdle(() =>
                {
                    var prompter = new SolutionPrompter(es);
                    prompter.Check(fileName);
                });
            }
        }
    }
}
