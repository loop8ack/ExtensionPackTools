using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features;

internal static class VisualStudioExtensions
{
    public static async Task<List<IVSExtension>> GetInstalledExtensionsAsync(this IVSExtensions extensions)
    {
        return (await extensions.GetInstalledExtensionsAsync().ConfigureAwait(false))
            .OrderBy(e => e.Name)
            .ToList();
    }

    public static async Task<string?> GetCurrentSolutionExtensionsManifestFilePathAsync(this IVSSolutions solutions, IVSMessageBox messageBox)
    {
        var solution = await solutions.GetCurrentOrThrowAsync();

        if (solution.Name is null or { Length: 0 })
        {
            await messageBox.ShowErrorAsync("The solution must be saved in order to manage solution extensions.").ConfigureAwait(false);

            return null;
        }

        return Path.ChangeExtension(solution.Name, ".vsext");
    }

    public static async Task<IVSSolution> GetCurrentOrThrowAsync(this IVSSolutions solutions)
    {
        return await solutions.GetCurrentSolutionAsync()
            ?? throw new InvalidOperationException("No solution is loaded");
    }
}
