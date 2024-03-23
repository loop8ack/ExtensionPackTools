using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Utils;

internal sealed class ExtensionEqualityComparer : IEqualityComparer<IVSExtension>
{
    public static ExtensionEqualityComparer Instance { get; } = new ExtensionEqualityComparer();

    public bool Equals(IVSExtension x, IVSExtension y)
         => x.Id == y.Id;

    public int GetHashCode(IVSExtension obj)
        => obj.Id.GetHashCode();
}
