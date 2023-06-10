using System.Windows;

using ExtensionManager.VisualStudio;

namespace ExtensionManager.UI.Attached;

internal static class VSTheme
{
    public static readonly DependencyProperty UseProperty;

    static VSTheme()
    {
        UseProperty = DependencyProperty.RegisterAttached(
            "Use",
            typeof(bool),
            typeof(VSTheme),
            new FrameworkPropertyMetadata(false, OnUsePropertyChanged)
            {
                Inherits = true
            });
    }

    public static void SetUse(UIElement element, bool value) => element.SetValue(UseProperty, value);
    public static bool GetUse(UIElement element) => (bool)element.GetValue(UseProperty);

    private static void OnUsePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!VSFacade.IsInitialized)
            return;

        if (d is not UIElement element)
            return;
        if (e.NewValue is not bool value)
            return;

        VSFacade.Themes.Use(element, value);
    }
}
