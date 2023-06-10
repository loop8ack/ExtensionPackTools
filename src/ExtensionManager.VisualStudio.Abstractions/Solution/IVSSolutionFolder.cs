using System.Threading.Tasks;

namespace ExtensionManager.VisualStudio.Solution;

/// <summary>
/// Represents a solution folder in the solution
/// </summary>
public interface IVSSolutionFolder : IVSSolutionItem
{
    /// <summary>
    /// Adds one or more files to the solution folder.
    /// </summary>
    Task AddExistingFilesAsync(params string[] files);
}
