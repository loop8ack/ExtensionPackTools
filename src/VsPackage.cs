using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Tasks = System.Threading.Tasks;

namespace ExtensionPackTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    public sealed class VsPackage : AsyncPackage
    {
        protected override async Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // Waits for MEF to initialize before the extension manager is ready to use
            await GetServiceAsync(typeof(SComponentModel));

            // Main thread required for registering the command
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (await GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                ExportCommand.Initialize(this, commandService);
            }
        }
    }
}
