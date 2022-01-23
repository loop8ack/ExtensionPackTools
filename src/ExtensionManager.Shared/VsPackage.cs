using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ExtensionManager.Core.Services.Interfaces;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Events;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace ExtensionManager
{
    [PackageRegistration(
        UseManagedResourcesOnly = true, AllowsBackgroundLoading = true
    )]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    [ProvideAutoLoad(
        VSConstants.UICONTEXT.SolutionOpening_string,
        PackageAutoLoadFlags.BackgroundLoad
    )]
    public sealed class VsPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            var manager =
                await GetServiceAsync(typeof(SVsExtensionManager)) as
                    IVsExtensionManager;
            var repository =
                await GetServiceAsync(typeof(SVsExtensionRepository)) as
                    IVsExtensionRepository;

            var solService =
                await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            var isSolutionLoaded = await IsSolutionLoadedAsync(solService);

            var extensionService = BuildNew
                                   .ExtensionService
                                   .UsingVsExtensionManager(manager)
                                   .AndVsExtensionRepository(repository);
            if (isSolutionLoaded)
            {
                JoinableTaskFactory.RunAsync(
                                       () => HandleOpenSolutionAsync(
                                           solService, extensionService,
                                           cancellationToken
                                       )
                                   )
                                   .FileAndForget(
                                       $"{nameof(ExtensionManager)}/{nameof(HandleOpenSolutionAsync)}"
                                   );
                ;
            }

            // Listen for subsequent solution events
            SolutionEvents.OnAfterOpenSolution += (s, e)
                => HandleOpenSolutionAsync(
                        solService, extensionService, cancellationToken
                    )
                    .ConfigureAwait(false);

            if (await GetServiceAsync(typeof(IMenuCommandService)) is
                OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(
                    this, commandService, extensionService
                );
                ExportSolutionCommand.Initialize(
                    this, commandService, extensionService
                );
                ImportCommand.Initialize(
                    this, commandService, extensionService
                );
            }
        }

        private async Task<bool> IsSolutionLoadedAsync(IVsSolution solService)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(
                solService.GetProperty(
                    (int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value
                )
            );

            return value is bool isSolOpen && isSolOpen;
        }

        private async Task HandleOpenSolutionAsync(IVsSolution solService,
            IExtensionService extensionService,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(
                solService.GetProperty(
                    (int)__VSPROPID.VSPROPID_SolutionFileName, out var value
                )
            );

            if (value is string solFileName &&
                !string.IsNullOrEmpty(solFileName))
            {
                var fileName = Path.ChangeExtension(solFileName, ".vsext");

                await ThreadHelper.JoinableTaskFactory.StartOnIdle(
                    () =>
                    {
                        var prompter = new SolutionPrompter(extensionService);
                        prompter.Check(fileName);
                    }
                );
            }
        }
    }
}