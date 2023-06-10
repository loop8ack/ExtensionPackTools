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
#elif V17_5
namespace ExtensionManager.VisualStudio.V17_5;
#elif V17_7
namespace ExtensionManager.VisualStudio.V17_7;
#endif

public sealed class VSFacadeImplementation : VSFacade
{
    public static void Initialize() => Initialize(new VSFacadeImplementation());

    protected override IVSThemes CreateThemes() => new VSThemes();
    protected override IVSThreads CreateThreads() => new VSThreads();
    protected override IVSSolutions CreateSolutions() => new VSSolutions();
    protected override IVSStatusBar CreateStatusBar() => new VSStatusBar();
    protected override IVSDocuments CreateDocuments() => new VSDocuments();
    protected override IVSMessageBox CreateMessageBox() => new VSMessageBox();
    protected override IVSExtensions CreateExtensions() => new VSExtensions();
}
