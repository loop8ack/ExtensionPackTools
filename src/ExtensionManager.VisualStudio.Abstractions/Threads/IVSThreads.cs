namespace ExtensionManager.VisualStudio.Threads;

/// <summary>
/// Contains methods for dealing with threads.
/// </summary>
public interface IVSThreads
{
    /// <summary>
    /// Determines if the call is being made on the UI thread.
    /// </summary>
    /// <returns><see langword="true"/> if the call is on the UI thread.</returns>
    bool CheckUIThreadAccess();

    /// <summary>
    /// Executes the method on the UI thread.
    /// </summary>
    Task RunOnUIThreadAsync(Action syncMethod);

    /// <summary>
    /// Executes the method on the UI thread.
    /// </summary>
    Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> syncMethod);
}
