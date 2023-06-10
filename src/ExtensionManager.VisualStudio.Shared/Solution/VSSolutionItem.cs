using CT = Community.VisualStudio.Toolkit;

namespace ExtensionManager.VisualStudio.Solution;

internal class VSSolutionItem<TItem> : IVSSolutionItem
    where TItem : CT.SolutionItem
{
    protected TItem Inner { get; }

    public string Name => Inner.Name;
    public string? FullPath => Inner.FullPath;

    protected VSSolutionItem(TItem inner)
        => Inner = inner;
}
