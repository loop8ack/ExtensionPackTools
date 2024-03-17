using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExtensionManager.VisualStudio.Extensions;

/// <summary> Contains methods for dealing with Visual Studio Extensions. </summary>
public interface IVSExtensions
{
    /// <summary>
    /// Retrieves the IDs of all installed extensions.
    /// </summary>
    Task<IReadOnlyCollection<IVSExtension>> GetInstalledExtensionsAsync();

    /// <summary>
    /// Downloads extension data based on the specified extension IDs
    /// </summary>
    Task<IReadOnlyCollection<IVSExtension>> GetGalleryExtensionsAsync(IEnumerable<string> extensionIds);

    /// <summary>
    /// Starts the VsixInstaller with the specified vsix files.
    /// </summary>
    Task StartInstallerAsync(IEnumerable<string> vsixFiles, bool systemWide);
}
