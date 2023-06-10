using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

using Community.VisualStudio.Toolkit;

using ExtensionManager.Features.Export;
using ExtensionManager.Features.Install;
using ExtensionManager.VisualStudio;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using Task = System.Threading.Tasks.Task;

#nullable enable

namespace ExtensionManager;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.guidVsPackageString)]
[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
public sealed class ExtensionManagerPackage : AsyncPackage
{
    static ExtensionManagerPackage()
        => AssemblyResolver.Initialize();

    // If a file is displayed by exporting the extensions, an empty solution is opened.
    // In this case, InstallSolutionExtensionsOnIdleAsync would be called, but this leads to an error because the solution has not yet been saved.
    private bool _isFeatureExecuting;

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        var services = new ServiceCollection()
            .ConfigureExtensionManager()
            .BuildServiceProvider();

        var featureExecutor = services.GetRequiredService<IFeatureExecutor>();

        await InitializeVSFacadeAsync();
        await InitMenuCommandsAsync(featureExecutor);
        await HandleSolutionExtensionsAsync(featureExecutor);
    }
    private static async Task InitializeVSFacadeAsync()
    {
        var vsVersion = await VS.Shell.GetVsVersionAsync();

#if V15
        if (vsVersion >= new Version(15, 0))
            VisualStudio.V15.VSFacadeImplementation.Initialize();
#elif V16
        if (vsVersion >= new Version(16, 0))
            VisualStudio.V16.VSFacadeImplementation.Initialize();
#elif V17
        if (vsVersion >= new Version(17, 7))
            VisualStudio.V17_7.VSFacadeImplementation.Initialize();
        else if (vsVersion >= new Version(17, 5))
            VisualStudio.V17_5.VSFacadeImplementation.Initialize();
#endif
    }

    private async Task HandleSolutionExtensionsAsync(IFeatureExecutor executor)
    {
        if (await VSFacade.Solutions.IsOpenAsync())
        {
            JoinableTaskFactory
                .RunAsync(() => InstallSolutionExtensionsOnIdleAsync(executor))
                .FileAndForget($"{nameof(ExtensionManager)}/{nameof(InstallSolutionExtensionsOnIdleAsync)}");
        }

        VS.Events.SolutionEvents.OnAfterOpenSolution += s =>
        {
            if (_isFeatureExecuting)
                return;

            _ = InstallSolutionExtensionsOnIdleAsync(executor);
        };

        static async Task InstallSolutionExtensionsOnIdleAsync(IFeatureExecutor executor)
        {
            await ThreadHelper.JoinableTaskFactory.StartOnIdle(
                executor.ExecuteAsync<InstallForSolutionFeature>);
        }
    }

    private async Task InitMenuCommandsAsync(IFeatureExecutor executor)
    {
        if (await GetServiceAsync(typeof(IMenuCommandService)) is not IMenuCommandService commandService)
            return;

        AddMenuCommand<ExportFeature>(executor, commandService, PackageGuids.guidExportPackageCmdSet, PackageIds.ExportCmd);
        AddMenuCommand<ExportSolutionFeature>(executor, commandService, PackageGuids.guidExportPackageCmdSet, PackageIds.ExportSolutionCmd);
        AddMenuCommand<InstallFeature>(executor, commandService, PackageGuids.guidExportPackageCmdSet, PackageIds.ImportCmd);
    }
    private void AddMenuCommand<TFeature>(IFeatureExecutor executor, IMenuCommandService commandService, Guid menuGroup, int commandID)
        where TFeature : class, IFeature
    {
        var cmdId = new CommandID(menuGroup, commandID);
        var cmd = new MenuCommand(OnHandleCommand, cmdId);
        commandService.AddCommand(cmd);

        [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods")]
        async void OnHandleCommand(object sender, EventArgs e)
        {
            _isFeatureExecuting = true;

            try
            {
                await executor.ExecuteAsync<TFeature>();
            }
            finally
            {
                _isFeatureExecuting = false;
            }
        }
    }
}

file static class AssemblyResolver
{
    public static void Initialize()
        => AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

    private static Assembly? OnAssemblyResolve(object sender, ResolveEventArgs e)
    {
        var assemblyName = new AssemblyName(e.Name).Name;
        var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var assemblyPath = Path.Combine(currentFolder, assemblyName + ".dll");

        if (File.Exists(assemblyPath))
            return Assembly.LoadFile(assemblyPath);

        return null;
    }
}
