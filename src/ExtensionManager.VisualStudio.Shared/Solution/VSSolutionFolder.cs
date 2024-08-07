using System.Threading.Tasks;

using CT = Community.VisualStudio.Toolkit;

#nullable enable

namespace ExtensionManager.VisualStudio.Solution;

internal sealed class VSSolutionFolder : VSSolutionItem<CT.SolutionFolder>, IVSSolutionFolder
{
    public VSSolutionFolder(CT.SolutionFolder folder)
        : base(folder)
    {
    }

    public Task AddExistingFilesAsync(params string[] files) => Inner.AddExistingFilesAsync(files);
}
