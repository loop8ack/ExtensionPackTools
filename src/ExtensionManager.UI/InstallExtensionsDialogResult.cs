using System.Collections.Generic;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI;

public sealed class InstallExtensionsDialogResult
{
    public bool SystemWide { get; }
    public IReadOnlyCollection<IVSExtension> Extensions { get; }

    internal InstallExtensionsDialogResult(bool systemWide, IReadOnlyCollection<IVSExtension> extensions)
    {
        SystemWide = systemWide;
        Extensions = extensions;
    }
}
