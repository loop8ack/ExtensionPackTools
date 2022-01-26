using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to set the theme of UI elements to match what Visual
    /// Studio is currently using.
    /// </summary>
    public static class VsTheme
    {
        /// <summary>
        /// Dictionary that keeps track of whether specific UI elements are using the
        /// current Visual Studio theme or not.
        /// </summary>
        private static readonly Dictionary<IInputElement, bool>
            _elementsUsingTheme = new Dictionary<IInputElement, bool>();

        /// <summary>
        /// Dictionary that keeps track of the original background colors (prior to
        /// theming) of controls.
        /// </summary>
        /// <remarks>
        /// The user may have set custom colors for Windows control for the
        /// operating system as a whole.
        /// <para />
        /// It's important to store the original background colors here, so that if the
        /// theme is unapplied from Visual Studio by the user, the controls revert to their
        /// colors as they were prior to the theme being applied.
        /// </remarks>
        private static readonly Dictionary<IInputElement, object>
            _originalBackgrounds = new Dictionary<IInputElement, object>();

        /// <summary>
        /// Gets or sets a value -- typically set in a window's XAML -- that indicates
        /// whether that window will respect the current theme colors.
        /// </summary>
        public static DependencyProperty UseVsThemeProperty =
            DependencyProperty.RegisterAttached(
                "UseVsTheme", typeof(bool), typeof(VsTheme),
                new PropertyMetadata(false, UseVsThemePropertyChanged)
            );

        /// <summary>
        /// Gets a reference to the <see cref="T:System.Windows.ResourceDictionary" /> that
        /// corresponds to the current Visual Studio theme.
        /// </summary>
        private static ResourceDictionary ThemeResources { get; } =
            BuildThemeResources();

        /// <summary>
        /// Called when the value of the <c>UseVsTheme</c> dependency property is changed.
        /// </summary>
        /// <param name="d">
        /// (Required.) Reference to the
        /// <see cref="T:System.Windows.DependencyObject" /> that the property has been
        /// changed for.
        /// </param>
        /// <param name="e">
        /// (Required.) A
        /// <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> that
        /// contains the event data.
        /// </param>
        /// <remarks>
        /// This method responds by checking whether <paramref name="d" /> is a
        /// <see cref="T:System.Windows.UIElement" />.  If not, then it quits.
        /// <para />
        /// Likewise, the method then checks whether the type of the
        /// <see cref="P:System.Windows.DependencyPropertyChangedEventArgs.NewValue" />
        /// property is <see cref="T:System.Boolean" />.  We expect this to be the case
        /// since it's the value that is supposed to indicate whether to apply the theme.
        /// <para />
        /// If this is not the case, then the execution of this method stops.
        /// <para />
        /// Otherwise, in the event that the arguments passed to this method contain the
        /// correct data, we then use the value of the
        /// <see cref="P:System.Windows.DependencyPropertyChangedEventArgs.NewValue" />
        /// property to decide whether or not to apply the theme to the
        /// <see cref="T:System.Windows.UIElement" />, a reference to which is passed in
        /// the argument of the <paramref name="d" /> parameter.
        /// </remarks>
        private static void UseVsThemePropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement element)) return;

            if (!(e.NewValue is bool value)) return;

            SetUseVsTheme(element, value);
        }

        /// <summary>
        /// Depending on whether the specified <paramref name="value" /> is
        /// <see langword="true" /> (apply theme) or <see langword="false" /> remove theme,
        /// applies or removes the Visual Studio theme, as directed, to the specified UI
        /// <paramref name="element" />.
        /// </summary>
        /// <param name="element">
        /// (Required.) Reference to an instance of an object that
        /// implements the <see cref="T:System.Windows.IInputElement" /> interface.
        /// <para />
        /// This object plays the role of the user-interface element that is being themed
        /// (or un-themed, such as the case may be).
        /// </param>
        /// <param name="value">
        /// (Required.) A <see cref="T:System.Boolean" /> value that
        /// determines whether to apply the theme (<see langword="true" />) or remove the
        /// theme (<see langword="false" />).
        /// </param>
        /// <remarks>
        /// This method will not execute if the specified
        /// <paramref name="element" /> is not a reference to a
        /// <see cref="T:System.Windows.Controls.Control" />.
        /// </remarks>
        public static void SetUseVsTheme(IInputElement element, bool value)
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

        /// <summary>
        /// For the specified <paramref name="element" />, determines whether the element
        /// is currently respecting the applied Visual Studio theme or not.
        /// </summary>
        /// <param name="element">
        /// (Required.) Reference to an instance of an object that
        /// implements the <see cref="T:System.Windows.IInputElement" /> interface.
        /// </param>
        /// <returns>
        /// If the specified <paramref name="element" /> is
        /// <see langword="null" />, then <see langword="false" /> is returned.  Likewise
        /// if the <see cref="F:ExtensionManager.VsTheme._elementsUsingTheme" /> dictionary
        /// has zero elements, or if the specified <paramref name="element" /> does not
        /// have an entry in said dictionary.
        /// <para />
        /// Otherwise, this method returns whatever the current value is set for the
        /// specified <paramref name="element" /> in the
        /// <see cref="F:ExtensionManager.VsTheme._elementsUsingTheme" /> dictionary.
        /// </returns>
        public static bool GetUseVsTheme(IInputElement element)
        {
            if (element == null) return false;
            if (!_elementsUsingTheme.Any()) return false;
            if (!_elementsUsingTheme.ContainsKey(element)) return false;

            return _elementsUsingTheme.TryGetValue(element, out var value) &&
                   value;
        }

        /// <summary>
        /// Builds a new <see cref="T:System.Windows.ResourceDictionary" /> based off the
        /// current Visual Studio theme colors.
        /// </summary>
        /// <returns>
        /// Reference to an instance of
        /// <see cref="T:System.Windows.ResourceDictionary" /> that corresponds to the
        /// currently-set theme in Visual Studio.
        /// </returns>
        /// <remarks>
        /// The return value of this method will be initialized to default values if, say,
        /// an
        /// exception was caught during processing.
        /// </remarks>
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
                allResources = new ResourceDictionary();
            }

            return allResources;
        }

        /// <summary>
        /// Applies the Visual Studio theme to the specified
        /// <paramref name="frameworkElement" /> in the user-interface.
        /// </summary>
        /// <param name="frameworkElement">
        /// (Required.) A
        /// <see cref="T:System.Windows.FrameworkElement" /> to which the current Visual
        /// Studio theme should be applied.
        /// </param>
        /// <remarks>
        /// This method does nothing if <paramref name="frameworkElement" /> is
        /// <see langword="null" />.
        /// </remarks>
        private static void ApplyTheme(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) return;

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

        /// <summary>
        /// Removes the Visual Studio theme to the specified
        /// <paramref name="frameworkElement" /> in the user-interface.
        /// </summary>
        /// <param name="frameworkElement">
        /// (Required.) A
        /// <see cref="T:System.Windows.FrameworkElement" /> to which the current Visual
        /// Studio theme should be applied.
        /// </param>
        /// <remarks>
        /// This method does nothing if <paramref name="frameworkElement" /> is
        /// <see langword="null" />.
        /// </remarks>
        private static void RemoveTheme(this FrameworkElement frameworkElement)
        {
            if (frameworkElement == null) return;

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
            if (!GetUseVsTheme(frameworkElement) ||
                !(frameworkElement is Control control)) return;

            if (_originalBackgrounds.TryGetValue(
                    frameworkElement, out var background
                ))
                control.SetValue(Control.BackgroundProperty, background);
            else
                control.ClearValue(Control.BackgroundProperty);
        }
    }
}