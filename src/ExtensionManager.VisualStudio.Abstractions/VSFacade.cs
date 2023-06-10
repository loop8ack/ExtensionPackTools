using System;
using System.Threading;

using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

namespace ExtensionManager.VisualStudio;

/// <summary>
/// This class is an abstraction around the Visual Studio API and should have the same look and feel as the Community.VisualStudio.Toolkit.VS class.
/// </summary>
public abstract class VSFacade
{
    private static VSFacade? _instance;
    private static IVSThemes? _themes;
    private static IVSThreads? _threads;
    private static IVSSolutions? _solutions;
    private static IVSStatusBar? _statusBar;
    private static IVSDocuments? _documents;
    private static IVSMessageBox? _messageBox;
    private static IVSExtensions? _extensions;

    /// <summary>
    /// Indicates whether this <see cref="VSFacade"/> is initialised or not. <br/>
    /// This should always be <see langword="true"/> at runtime and <see langword="false"/> at design time.
    /// </summary>
    public static bool IsInitialized => _instance is not null;

    /// <summary>Contains methods for WPF to deal with Visual Studio Themes.</summary>
    public static IVSThemes Themes => _themes ??= Instance.CreateThemes();

    /// <summary> Contains methods for dealing with threads. </summary>
    public static IVSThreads Threads => _threads ??= Instance.CreateThreads();

    /// <summary> A collection of services related to solutions. </summary>
    public static IVSSolutions Solutions => _solutions ??= Instance.CreateSolutions();

    /// <summary> An API wrapper that makes it easy to work with the status bar. </summary>
    public static IVSStatusBar StatusBar => _statusBar ??= Instance.CreateStatusBar();

    /// <summary> Contains helper methods for dealing with documents. </summary>
    public static IVSDocuments Documents => _documents ??= Instance.CreateDocuments();

    /// <summary> Shows message boxes. </summary>
    public static IVSMessageBox MessageBox => _messageBox ??= Instance.CreateMessageBox();

    /// <summary> Contains methods for dealing with Visual Studio Extensions. </summary>
    public static IVSExtensions Extensions => _extensions ??= Instance.CreateExtensions();

    private static VSFacade Instance
    {
        get => _instance ?? throw new InvalidOperationException("VS facade was not already initialized");
    }

    protected static void Initialize(VSFacade instance)
    {
        var oldInstance = Interlocked.CompareExchange(ref _instance, instance, null);

        if (oldInstance is not null)
            throw new InvalidOperationException("VS facade was already initialized");
    }

    protected VSFacade() { }

    protected abstract IVSThemes CreateThemes();
    protected abstract IVSThreads CreateThreads();
    protected abstract IVSSolutions CreateSolutions();
    protected abstract IVSStatusBar CreateStatusBar();
    protected abstract IVSDocuments CreateDocuments();
    protected abstract IVSMessageBox CreateMessageBox();
    protected abstract IVSExtensions CreateExtensions();
}
