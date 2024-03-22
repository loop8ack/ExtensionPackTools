namespace ExtensionManager.VisualStudio.Solution;

/// <summary>
/// A collection of services related to solutions.
/// </summary>
public interface IVSSolutions
{
    /// <summary>
    /// Checks if a solution is open.
    /// </summary>
    Task<bool> IsOpenAsync();

    /// <summary>
    /// Gets the current solution.
    /// </summary>
    Task<IVSSolution?> GetCurrentSolutionAsync();
}
