using CT = Community.VisualStudio.Toolkit;

namespace ExtensionManager.VisualStudio.Solution;

internal sealed class VSSolution : VSSolutionItem<CT.Solution>, IVSSolution
{
    public VSSolution(CT.Solution solution)
        : base(solution)
    {
    }

    public async Task<IVSSolutionFolder?> AddSolutionFolderAsync(string name)
    {
        var folder = await Inner.AddSolutionFolderAsync(name).ConfigureAwait(false);

        if (folder is null)
            return null;

        return new VSSolutionFolder(folder);
    }
}
