namespace ExtensionManager.VisualStudio.Extensions;

/// <summary>
/// Represents a Visual Studio extension.
/// </summary>
public interface IVSExtension
{
    /// <summary>
    /// Gets the unique identifier for the extension.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the name of the extension.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the URL for more information about the extension.
    /// </summary>
    string? MoreInfoURL { get; }

    /// <summary>
    /// Gets the URL to download the extension.
    /// </summary>
    string? DownloadUrl { get; }
}
