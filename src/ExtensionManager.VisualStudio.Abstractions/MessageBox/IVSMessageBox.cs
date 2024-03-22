namespace ExtensionManager.VisualStudio.MessageBox;

/// <summary>
/// Shows message boxes.
/// </summary>
public interface IVSMessageBox
{
    /// <summary>
    /// Shows an error message box.
    /// </summary>
    Task ShowErrorAsync(string line1, string line2 = "");

    /// <summary>
    /// Shows a warning message box.
    /// </summary>
    /// <returns><see langword="true"/> if the OK button was clicked, <see langword="false"/> otherwise.</returns>
    Task<bool> ShowWarningAsync(string line1, string line2 = "");
}
