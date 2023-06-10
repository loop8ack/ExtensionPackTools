using System.Threading.Tasks;

namespace ExtensionManager.VisualStudio.Solution;

/// <summary>
/// Represents the solution itself.
/// </summary>
public interface IVSSolution : IVSSolutionItem
{
    /// <summary>
    /// Adds a solution folder to the solution
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IVSSolutionFolder?> AddSolutionFolderAsync(string name);
}
