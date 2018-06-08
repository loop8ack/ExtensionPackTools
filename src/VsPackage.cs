using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Tasks = System.Threading.Tasks;

namespace ExtensionPackTools
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    public sealed class VsPackage : AsyncPackage
    {
        protected override async Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(this, commandService);
            }
        }
    }
}
