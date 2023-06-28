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
public interface IVSFacade
{
    /// <summary>Contains methods for WPF to deal with Visual Studio Themes.</summary>
    IVSThemes Themes { get; }

    /// <summary> Contains methods for dealing with threads. </summary>
    IVSThreads Threads { get; }

    /// <summary> A collection of services related to solutions. </summary>
    IVSSolutions Solutions { get; }

    /// <summary> An API wrapper that makes it easy to work with the status bar. </summary>
    IVSStatusBar StatusBar { get; }

    /// <summary> Contains helper methods for dealing with documents. </summary>
    IVSDocuments Documents { get; }

    /// <summary> Shows message boxes. </summary>
    IVSMessageBox MessageBox { get; }

    /// <summary> Contains methods for dealing with Visual Studio Extensions. </summary>
    IVSExtensions Extensions { get; }
}
