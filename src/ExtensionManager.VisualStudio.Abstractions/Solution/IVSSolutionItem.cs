
namespace ExtensionManager.VisualStudio.Solution;

/// <summary>
/// Represents a file, folder, project, or other item in Solution Explorer.
/// </summary>
public interface IVSSolutionItem
{
    /// <summary>
    /// The name of the item.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The absolute file path on disk.
    /// </summary>
    string? FullPath { get; }

    /// <summary>
    /// Gets the children of this solution item.
    /// </summary>
    Task<IReadOnlyList<IVSSolutionItem>> GetChildrenAsync();
}
