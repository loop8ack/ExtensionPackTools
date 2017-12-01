using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace ExtensionPackTools
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidVsPackageString)]
    public sealed class VsPackage : Package
    {
        protected override void Initialize()
        {
            ExportCommand.Initialize(this);
        }
    }
}
