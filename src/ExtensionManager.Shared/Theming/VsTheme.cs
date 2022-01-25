using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public static class VsTheme
    {
        private static readonly Dictionary<UIElement, bool> _elementsUsingTheme =
            new Dictionary<UIElement, bool>();

        private static readonly Dictionary<UIElement, object>
            _originalBackgrounds = new Dictionary<UIElement, object>();

        public static DependencyProperty UseVsThemeProperty =
            DependencyProperty.RegisterAttached(
                "UseVsTheme", typeof(bool), typeof(VsTheme),
                new PropertyMetadata(false, UseVsThemePropertyChanged)
            );

        private static ResourceDictionary ThemeResources { get; } =
            BuildThemeResources();

        private static void UseVsThemePropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element)) return;
            if (!(e.NewValue is bool value)) return;

            SetUseVsTheme(element, value);
        }

        public static void SetUseVsTheme(UIElement element, bool value)
        {
            if (!(element is Control control)) return;

            if (value)
            {
                if (!_originalBackgrounds.ContainsKey(element))
                    _originalBackgrounds[element] = control.Background;

                control.ApplyTheme();
            }
            else
            {
                control.RemoveTheme();
            }

            _elementsUsingTheme[element] = value;
        }

        public static bool GetUseVsTheme(UIElement element)
        {
            if (element == null) return false;
            if (!_elementsUsingTheme.Any()) return false;
            if (!_elementsUsingTheme.ContainsKey(element)) return false;
            
            return _elementsUsingTheme.TryGetValue(element, out var value) && value;
        }

        private static ResourceDictionary BuildThemeResources()
        {
            var allResources = new ResourceDictionary();

            try
            {
                var shellResources =
                    (ResourceDictionary)Application.LoadComponent(
                        new Uri(
                            "Microsoft.VisualStudio.Platform.WindowManagement;component/Themes/ThemedDialogDefaultStyles.xaml",
                            UriKind.Relative
                        )
                    );
                var scrollStyleContainer =
                    (ResourceDictionary)Application.LoadComponent(
                        new Uri(
                            "Microsoft.VisualStudio.Shell.UI.Internal;component/Styles/ScrollBarStyle.xaml",
                            UriKind.Relative
                        )
                    );
                allResources.MergedDictionaries.Add(shellResources);
                allResources.MergedDictionaries.Add(scrollStyleContainer);
                allResources[typeof(ScrollViewer)] = new Style {
                    TargetType = typeof(ScrollViewer),
                    BasedOn =
                        (Style)scrollStyleContainer[
                            VsResourceKeys.ScrollViewerStyleKey]
                };

                allResources[typeof(TextBox)] = new Style {
                    TargetType = typeof(TextBox),
                    BasedOn = (Style)shellResources[typeof(TextBox)],
                    Setters = {
                        new Setter(
                            Control.PaddingProperty, new Thickness(2, 3, 2, 3)
                        )
                    }
                };

                allResources[typeof(ComboBox)] = new Style {
                    TargetType = typeof(ComboBox),
                    BasedOn = (Style)shellResources[typeof(ComboBox)],
                    Setters = {
                        new Setter(
                            Control.PaddingProperty, new Thickness(2, 3, 2, 3)
                        )
                    }
                };
            }
            catch
            {
                // ignored
            }

            return allResources;
        }

        private static void ApplyTheme(this FrameworkElement frameworkElement)
        {
            if (frameworkElement.Resources != ThemeResources)
            {
                var resourceDictionary = new ResourceDictionary();
                resourceDictionary.MergedDictionaries.Add(ThemeResources);
                resourceDictionary.MergedDictionaries.Add(
                    frameworkElement.Resources
                );
                frameworkElement.Resources = null;
                frameworkElement.Resources = resourceDictionary;
            }

            if (frameworkElement is Control control)
                control.SetResourceReference(
                    Control.BackgroundProperty,
                    (string)EnvironmentColors.StartPageTabBackgroundBrushKey
                );
        }

        private static void RemoveTheme(this FrameworkElement frameworkElement)
        {
            if (frameworkElement.Resources != null)
            {
                if (frameworkElement.Resources == ThemeResources)
                    frameworkElement.Resources = new ResourceDictionary();
                else
                    frameworkElement.Resources.MergedDictionaries.Remove(
                        ThemeResources
                    );
            }

            //If we're themed now and we're something with a background property, reset it
            if (GetUseVsTheme(frameworkElement) &&
                frameworkElement is Control c)
            {
                if (_originalBackgrounds.TryGetValue(
                        frameworkElement, out var background
                    ))
                    c.SetValue(Control.BackgroundProperty, background);
                else
                    c.ClearValue(Control.BackgroundProperty);
            }
        }
    }
}