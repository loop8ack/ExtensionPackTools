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

            var commandLine =
                await GetServiceAsync(typeof(SVsAppCommandLine)) as
                    IVsAppCommandLine;

            var isSolutionLoaded = await IsSolutionLoadedAsync(solService);

            var extensionService = MakeNew
                                   .ExtensionService
                                   .HavingVsExtensionManager(manager)
                                   .AndVsExtensionRepository(repository);
            var commandLineService =
                MakeNew.CommandLineService.HavingCommandLine(commandLine);

            if (isSolutionLoaded)
                JoinableTaskFactory.RunAsync(
                                       () => HandleOpenSolutionAsync(
                                           solService, extensionService,
                                           cancellationToken
                                       )
                                   )
                                   .FileAndForget(
                                       $"{nameof(ExtensionManager)}/{nameof(HandleOpenSolutionAsync)}"
                                   );

            // Listen for subsequent solution events
            SolutionEvents.OnAfterOpenSolution += (s, e)
                => HandleOpenSolutionAsync(
                        solService, extensionService, cancellationToken
                    )
                    .ConfigureAwait(false);

            if (!(await GetServiceAsync(typeof(OleMenuCommandService)) is
                    IMenuCommandService commandService)) return;

            InitializeCommands(
                commandService, extensionService, commandLineService
            );
        }

        /// <summary>
        /// Initializes the <c>*Command</c> classes.
        /// </summary>
        /// <param name="commandService">
        /// (Required.) Reference to an instance of an object
        /// that implements the
        /// <see cref="T:System.ComponentModel.Design.IMenuCommandService" /> interface.
        /// </param>
        /// <param name="extensionService">
        /// (Required.) Reference to an instance of an
        /// object that implements the <see cref="T:ExtensionManager.IExtensionService" />
        /// interface.
        /// </param>
        /// <param name="commandLineService">
        /// (Required.) Reference to an instance of an
        /// object that implements the
        /// <see cref="T:ExtensionManager.ICommandLineService" /> interface.
        /// </param>
        /// <remarks>
        /// If any one of the <paramref name="commandService" />,
        /// <paramref name="extensionService" />, or <paramref name="commandLineService" />
        /// parameters is passed <see langword="null" /> as its argument, then this method
        /// does nothing.
        /// </remarks>
        private void InitializeCommands(IMenuCommandService commandService,
            IExtensionService extensionService,
            ICommandLineService commandLineService)
        {
            /*
             * If the corresponding *service parameter is a null reference,
             * we must have failed to retrieve it in the VsPackage constructor.
             *
             * In this event, if any one of the parameters is null, it's useless to
             * proceed further.
             */

            if (commandService == null) return;
            if (extensionService == null) return;
            if (commandLineService == null) return;

            ExportCommand.Initialize(
                this, commandService, extensionService, commandLineService
            );
            ExportSolutionCommand.Initialize(
                this, commandService, extensionService, commandLineService
            );
            ImportCommand.Initialize(
                this, commandService, extensionService, commandLineService
            );
        }

        private async Task<bool> IsSolutionLoadedAsync(IVsSolution solService)
        {
            if (solService == null) return false;

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(
                solService.GetProperty(
                    (int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value
                )
            );

            return value is bool isSolOpen && isSolOpen;
        }

        /// <summary>
        /// In the event that we are processing a solution that has a <c>.vsext</c> file
        /// riding alongside it, then we should prompt the user to install required
        /// extensions upon the opening of the solution.
        /// <para />
        /// This method first checks whether this is the case, and, if so, then carries
        /// these actions out.
        /// </summary>
        /// <param name="solService">
        /// (Required.) Reference to an instance of an object that
        /// implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsSolution" /> interface.
        /// </param>
        /// <param name="extensionService">
        /// (Required.) Reference to an instance of an
        /// object that implements the <see cref="T:ExtensionManager.IExtensionService" />
        /// interface.
        /// </param>
        /// <param name="cancellationToken">
        /// (Optional.) A
        /// <see cref="T:System.Threading.CancellationToken" /> that makes this operation
        /// cancellable.
        /// </param>
        /// <remarks>
        /// If the arguments of either of the <paramref name="solService" /> or
        /// <paramref name="extensionService" /> parameters are <see langword="null" />,
        /// then this method does nothing.
        /// </remarks>
        private async Task HandleOpenSolutionAsync(IVsSolution solService,
            IExtensionService extensionService,
            CancellationToken cancellationToken)
        {
            if (solService == null) return;
            if (extensionService == null) return;

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