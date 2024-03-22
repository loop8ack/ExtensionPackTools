using System;
using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using EnvDTE80;

using Microsoft;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#if VS2017
namespace ExtensionManager.VisualStudio.VS2017;
#elif VS2019
namespace ExtensionManager.VisualStudio.VS2019;
#elif VS2022
namespace ExtensionManager.VisualStudio.VS2022;
#endif

public sealed class VisualStudioServices : IVisualStudioServices
{
    public static async Task<IVisualStudioServices> CreateAsync(AsyncPackage package, string vsixName, CancellationToken cancellationToken)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var repository = await package.GetServiceAsync<SVsExtensionRepository, IVsExtensionRepository>(cancellationToken);
        var vsCommandLine = await package.GetServiceAsync<SVsAppCommandLine, IVsAppCommandLine>(cancellationToken);
        var manager = await package.GetServiceAsync<SVsExtensionManager, IVsExtensionManager>(cancellationToken);
        var dte = await package.GetServiceAsync<DTE, DTE2>(cancellationToken);

        return new VisualStudioServices(package, repository, manager, vsCommandLine, dte, vsixName);
    }

    public IVisualStudioUIThread UIThread { get; }
    public IVisualStudioViewColors ViewColors { get; }
    public IVisualStudioViewResourceKeys ViewResourceKeys { get; }
    public IVisualStudioShell Shell { get; }
    public IVisualStudioSolution Solution { get; }
    public IVisualStudioExtensions Extensions { get; }
    public IVisualStudioInstance Instance { get; }

    private VisualStudioServices(IServiceProvider serviceProvider, IVsExtensionRepository repository, IVsExtensionManager manager, IVsAppCommandLine vsCommandLine, DTE2 dte, string vsixName)
    {
        UIThread = new Core.VisualStudioUIThread();
        ViewColors = new Core.VisualStudioViewColors();
        ViewResourceKeys = new Core.VisualStudioViewResourceKeys();
        Shell = new Core.VisualStudioShell(serviceProvider, dte, vsixName);
        Solution = new Core.VisualStudioSolution(Shell, dte.Solution as Solution2);
        Extensions = new Core.VisualStudioExtensions(repository, manager);
        Instance = new Core.VisualStudioInstance(vsCommandLine);
    }
}

file static class Extensions
{
    public static async Task<TInterface> GetServiceAsync<TService, TInterface>(this AsyncPackage package, CancellationToken cancellationToken)
        where TInterface : class
    {
        cancellationToken.ThrowIfCancellationRequested();

        var service = await package.GetServiceAsync(typeof(TService)) as TInterface;

        Assumes.Present(service);

        return service!;
    }
}
