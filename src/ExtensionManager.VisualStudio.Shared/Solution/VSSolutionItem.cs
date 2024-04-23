using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CT = Community.VisualStudio.Toolkit;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

#nullable enable

namespace ExtensionManager.VisualStudio.Solution;

internal class VSSolutionItem<TItem> : IVSSolutionItem
    where TItem : CT.SolutionItem
{
    protected TItem Inner { get; }

    public string Name => Inner.Name;
    public string? FullPath => Inner.FullPath;

    protected VSSolutionItem(TItem inner)
        => Inner = inner;

    public async Task<IReadOnlyList<IVSSolutionItem>> GetChildrenAsync()
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        return Inner.Children
            .Where(x => x is not null)
            .Select(x => (IVSSolutionItem)(x switch
            {
                CT.Solution solution => new VSSolution(solution),
                CT.SolutionFolder folder => new VSSolutionFolder(folder),
                CT.SolutionItem item => new VSSolutionItem<CT.SolutionItem>(item),
                _ => throw new NotImplementedException($"The solution item of type {x!.GetType()} is not supported"),
            }))
            .ToList();
    }
}
