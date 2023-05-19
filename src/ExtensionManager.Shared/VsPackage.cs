using System;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio;
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

            var vsVersion = await GetVisualStudioVersionAsync();
            var vsService = await VisualStudioServiceFactory.CreateAsync(this, vsVersion);

            var extService = new ExtensionService(vsService);
            var solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;

            var isSolutionLoaded = await IsSolutionLoadedAsync(solService);

            if (isSolutionLoaded)
            {
                JoinableTaskFactory.RunAsync(() =>
                {
                    return HandleOpenSolutionAsync(solService, extService, vsService, cancellationToken);
                }).FileAndForget($"{nameof(ExtensionManager)}/{nameof(HandleOpenSolutionAsync)}");
                ;
            }

            // Listen for subsequent solution events
            SolutionEvents.OnAfterOpenSolution += (s, e) =>
            {
                _ = HandleOpenSolutionAsync(solService, extService, vsService, cancellationToken);
            };

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(this, commandService, extService);
                ExportSolutionCommand.Initialize(this, commandService, extService);
                ImportCommand.Initialize(this, commandService, extService, vsService);
            }
        }

        private async Task<Version> GetVisualStudioVersionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var shell = await GetServiceAsync(typeof(SVsShell)) as IVsShell;
            shell.GetProperty((int)__VSSPROPID5.VSSPROPID_ReleaseVersion, out var versionObject);
            var version = versionObject.ToString();
            return Version.Parse(version.Substring(0, version.IndexOf(' ')));
        }

        private async Task<bool> IsSolutionLoadedAsync(IVsSolution solService)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value));

            return value is bool isSolOpen && isSolOpen;
        }

        private async Task HandleOpenSolutionAsync(IVsSolution solService, ExtensionService es, IVisualStudioService vsService, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_SolutionFileName, out var value));

            if (value is string solFileName && !string.IsNullOrEmpty(solFileName))
            {
                var fileName = Path.ChangeExtension(solFileName, ".vsext");

                await ThreadHelper.JoinableTaskFactory.StartOnIdle(() =>
                {
                    var prompter = new SolutionPrompter(this, es, vsService);
                    prompter.Check(fileName);
                });
            }
        }
    }
}
