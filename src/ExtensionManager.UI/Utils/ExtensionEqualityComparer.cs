using System.Collections.Generic;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI.Utils;

internal sealed class ExtensionEqualityComparerById : IEqualityComparer<IVSExtension>
{
    public static ExtensionEqualityComparerById Instance { get; } = new();

    public bool Equals(IVSExtension x, IVSExtension y)
        => x?.Id == y?.Id;

    public int GetHashCode(IVSExtension obj)
        => obj?.Id?.GetHashCode() ?? 0;
}
