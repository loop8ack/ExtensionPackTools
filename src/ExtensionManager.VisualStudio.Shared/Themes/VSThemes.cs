using System.Windows;

using CT = Community.VisualStudio.Toolkit;

namespace ExtensionManager.VisualStudio.Themes;

internal sealed class VSThemes : IVSThemes
{
    public void Use(UIElement element, bool use = true) => CT.Themes.SetUseVsTheme(element, use);
}
