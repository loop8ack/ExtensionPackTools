using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

#if V15
namespace ExtensionManager.VisualStudio.V15;
#elif V16
namespace ExtensionManager.VisualStudio.V16;
#elif V17
namespace ExtensionManager.VisualStudio.V17;
#elif V17_Preview
namespace ExtensionManager.VisualStudio.V17_Preview;
#endif

public sealed class VSServiceFactory : IVSServiceFactory
{
    public IVSThemes CreateThemes() => new VSThemes();
    public IVSThreads CreateThreads() => new VSThreads();
    public IVSSolutions CreateSolutions() => new VSSolutions();
    public IVSStatusBar CreateStatusBar() => new VSStatusBar();
    public IVSDocuments CreateDocuments() => new VSDocuments();
    public IVSMessageBox CreateMessageBox() => new VSMessageBox();
    public IVSExtensions CreateExtensions() => new VSExtensions();
}
