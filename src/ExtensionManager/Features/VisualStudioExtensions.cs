using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features;

internal static class VisualStudioExtensions
{
    public static async Task<IReadOnlyCollection<IVSExtension>> GetInstalledExtensionsAsync(this IVSExtensions extensions)
    {
        var installedIds = await extensions.GetInstalledExtensionIdsAsync().ConfigureAwait(false);

        // Filter the installed extensions to only be the ones that exist on the Marketplace
        return (await extensions.GetGalleryExtensionsAsync(installedIds).ConfigureAwait(false))
            .OrderBy(e => e.Name)
            .ToArray();
    }

    public static async Task<string?> GetCurrentSolutionExtensionsManifestFilePathAsync(this IVSSolutions solutions)
    {
        var solution = await solutions.GetCurrentOrThrowAsync();

        if (solution.Name is null or { Length: 0 })
        {
            await VSFacade.MessageBox.ShowErrorAsync("The solution must be saved in order to manage solution extensions.").ConfigureAwait(false);

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
