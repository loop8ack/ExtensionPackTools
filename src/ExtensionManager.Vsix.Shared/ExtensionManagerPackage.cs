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
using ExtensionManager.UI;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Solution;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using ShellSolutionEvents = Microsoft.VisualStudio.Shell.Events.SolutionEvents;
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
        var vsVersion = await VS.Shell.GetVsVersionAsync()
            ?? throw new InvalidOperationException("Cannot find running visual studio version");

        var services = new ServiceCollection()
            .ConfigureVSServices(CreateVSServiceFactory(vsVersion))
            .ConfigureExtensionManager(new ThisVsixInfo())
            .BuildServiceProvider();

        UIMarkupServices.Initialize(services);

        var solutions = services.GetRequiredService<IVSSolutions>();
        var featureExecutor = services.GetRequiredService<IFeatureExecutor>();

        await InitMenuCommandsAsync(featureExecutor);
        await HandleSolutionExtensionsAsync(solutions, featureExecutor);
    }

    private static IVSServicesRegistrar CreateVSServiceFactory(Version vsVersion)
    {
#if VS2022
        return new VisualStudio.VS2022.VSServicesRegistrar(vsVersion);
#elif VS2019
        return new VisualStudio.VS2019.VSServicesRegistrar(vsVersion);
#elif VS2017
        return new VisualStudio.VS2017.VSServicesRegistrar(vsVersion);
#else
#error Not implemented
#endif
    }

    private async Task HandleSolutionExtensionsAsync(IVSSolutions solutions, IFeatureExecutor executor)
    {
        if (await solutions.IsOpenAsync())
            InstallSolutionExtensionsOnIdle(executor);

        ShellSolutionEvents.OnAfterOpenSolution += (s, e) =>
        {
            if (_isFeatureExecuting)
                return;

            InstallSolutionExtensionsOnIdle(executor);
        };

        void InstallSolutionExtensionsOnIdle(IFeatureExecutor executor)
        {
            JoinableTaskFactory
                .StartOnIdle(executor.ExecuteAsync<InstallForSolutionFeature>)
                .FileAndForget($"{nameof(ExtensionManager)}/{nameof(InstallForSolutionFeature)}");
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
