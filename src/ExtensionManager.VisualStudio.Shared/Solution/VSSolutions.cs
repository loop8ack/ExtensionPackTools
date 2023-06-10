using System.Threading.Tasks;

using VS = Community.VisualStudio.Toolkit.VS;

namespace ExtensionManager.VisualStudio.Solution;

internal sealed class VSSolutions : IVSSolutions
{
    public Task<bool> IsOpenAsync() => VS.Solutions.IsOpenAsync();

    public async Task<IVSSolution?> GetCurrentSolutionAsync()
    {
        var solution = await VS.Solutions.GetCurrentSolutionAsync().ConfigureAwait(false);

        if (solution is null)
            return null;

        return new VSSolution(solution);
    }
}
