namespace ExtensionManager.VisualStudio.Documents;

/// <summary>
/// Contains helper methods for dealing with documents.
/// </summary>
public interface IVSDocuments
{
    /// <summary>
    /// Opens a file in editor window.
    /// </summary>
    Task OpenAsync(string file);
}
