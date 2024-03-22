namespace ExtensionManager.VisualStudio.StatusBar;

/// <summary>
/// An API wrapper that makes it easy to work with the status bar.
/// </summary>
public interface IVSStatusBar
{
    /// <summary>
    /// Clears all text from the status bar.
    /// </summary>
    Task ClearAsync();

    /// <summary>
    /// Sets the text in the status bar.
    /// </summary>
    Task ShowMessageAsync(string text);

    /// <summary>
    /// Shows the progress indicator in the status bar. 
    /// Set <paramref name="currentStep"/> and <paramref name="numberOfSteps"/> 
    /// to the same value to stop the progress.
    /// </summary>
    /// <param name="text">The text to display in the status bar.</param>
    /// <param name="currentStep">The current step number starting at 1.</param>
    /// <param name="numberOfSteps">The total number of steps to completion.</param>
    Task ShowProgressAsync(string text, int currentStep, int numberOfSteps);
}
