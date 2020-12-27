using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Window = System.Windows.Window;

namespace ExtensionManager.Importer
{
    public partial class ImportWindow : Window
    {
        private readonly IEnumerable<Extension> _extensions;
        private readonly Purpose _purpose;

        public ImportWindow(IEnumerable<Extension> extensions, Purpose purpose,
            string text = null)
        {
            _extensions = extensions;
            _purpose = purpose;
            Loaded += ImportWindow_Loaded;
            InitializeComponent();
            Title = Vsix.Name;

            if (!string.IsNullOrEmpty(text)) lblMessage.Content = text;

            btnOk.Content =
                purpose == Purpose.Install ? "Install..." : "Select";
            chkInstallSystemWide.Visibility = purpose == Purpose.Install
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        public List<Extension> SelectedExtension { get; private set; }

        public bool InstallSystemWide { get; private set; }

        private void ImportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hasCategory = false;
            var sortedList = _extensions;

            if (_purpose == Purpose.Install)
                sortedList = _extensions.OrderByDescending(x => x.Selected)
                    .ThenBy(x => x.Name);

            if (_extensions.FirstOrDefault()?.Selected == true)
            {
                var label = new Label {
                    Content = "Extensions",
                    Margin = new Thickness(0, 0, 0, 5),
                    FontWeight = FontWeights.Bold
                };

                list.Children.Add(label);
            }

            foreach (var ext in sortedList)
            {
                var cb = new CheckBox {
                    Content = ext.Name,
                    IsChecked = ext.Selected,
                    CommandParameter = ext.ID,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                if (_purpose == Purpose.Install && !ext.Selected)
                {
                    if (!hasCategory)
                    {
                        hasCategory = true;

                        var label = new Label {
                            Content = "Already installed",
                            Margin = new Thickness(0, 10, 0, 5),
                            FontWeight = FontWeights.Bold
                        };

                        list.Children.Add(label);
                    }

                    cb.IsEnabled = false;
                }

                list.Children.Add(cb);
            }
        }

        private void chkSelectDeselectAll_CheckedChanged(object sender,
            RoutedEventArgs e)
        {
            foreach (var cb in list.Children.OfType<CheckBox>())
                cb.IsChecked = chkSelectDeselectAll.IsChecked;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedExtension = new List<Extension>();

            foreach (var cb in list.Children.OfType<CheckBox>())
                if (cb.IsChecked == true && cb.IsEnabled)
                    SelectedExtension.Add(
                        _extensions.First(
                            ext => ext.ID == (string)cb.CommandParameter
                        )
                    );

            InstallSystemWide = chkInstallSystemWide.IsChecked ?? false;
            DialogResult = true;
            Close();
        }

        public static ImportWindow Open(IEnumerable<Extension> extensions,
            Purpose purpose, string msg = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            var dialog = new ImportWindow(extensions, purpose, msg);
            var hwnd = new IntPtr(dte.MainWindow.HWnd);

            var window = (Window)HwndSource.FromHwnd(hwnd).RootVisual;

            dialog.Owner = window;
            dialog.ShowDialog();

            return dialog;
        }
    }

    public enum Purpose
    {
        Install,
        List
    }
}