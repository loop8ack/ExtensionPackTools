using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

namespace ExtensionManager.VisualStudio;

public interface IVSServiceFactory
{
    IVSThemes CreateThemes();
    IVSThreads CreateThreads();
    IVSSolutions CreateSolutions();
    IVSStatusBar CreateStatusBar();
    IVSDocuments CreateDocuments();
    IVSMessageBox CreateMessageBox();
    IVSExtensions CreateExtensions();
}
