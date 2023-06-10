using System.Windows;

namespace ExtensionManager.VisualStudio.Themes;

/// <summary> Contains methods for WPF to deal with Visual Studio Themes. </summary>
public interface IVSThemes
{
    /// <summary>
    /// Sets a value that enables or disables whether each XAML control or window should be styled automatically using the VS theme properties.
    /// </summary>
    void Use(UIElement element, bool use = true);
}
