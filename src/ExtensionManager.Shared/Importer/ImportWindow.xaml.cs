using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager.Importer
{
    public partial class ImportWindow : Window
    {
        private readonly IEnumerable<Extension> _extensions;
        private readonly Purpose _purpose;

        public ImportWindow(IEnumerable<Extension> extensions, Purpose purpose, string text = null)
        {
            _extensions = extensions;
            _purpose = purpose;
            Loaded += ImportWindow_Loaded;
            InitializeComponent();

            // Set the OK button to have the proper verb for its text given the purpose
            btnOk.Content = purpose == Purpose.InstallForSolution ? "&Install" : purpose == Purpose.Import ? "&Import" : "&Export";

            // Show the Install System-Wide check box only if we are importing and/or
            // loading a solution
            chkInstallSystemWide.Visibility = purpose == Purpose.Import || purpose == Purpose.InstallForSolution 
                ? Visibility.Visible : Visibility.Hidden;

            InitializeWindowTitle(purpose);
            InitializeMainInstructionText(purpose);
            InitializeMessage(purpose, text);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            InitializeWindowChrome();
        }

        private void InitializeMessage(Purpose purpose, string text)
        {
            switch(purpose)
            {
                case Purpose.Export:
                    lblMessage.Content = "Select the extension(s) you wish to export.  Only extensions published to the Visual Studio Marketplace are shown.";
                    break;

                case Purpose.Import:
                    lblMessage.Content = "Select the extension(s) you wish to import.  Only extensions published to the Visual Studio Marketplace are shown.";
                    break;

                case Purpose.InstallForSolution:
                    if (!string.IsNullOrWhiteSpace(text))
                        lblMessage.Content = text;
                    break;

            }
        }

        private void InitializeMainInstructionText(Purpose purpose)
        {
            switch (purpose)
            {
                case Purpose.Export:
                    lblMainInstruction.Content = "Export your extension(s) to a file";
                    break;

                case Purpose.Import:
                    lblMainInstruction.Content = "Import extension(s) into Visual Studio";
                    break;

                case Purpose.InstallForSolution:
                    lblMainInstruction.Content = "Install extension(s) required by this solution";
                    break;
            }
        }

        /// <summary>
        /// Alters the title of the dialog box to ensure the correct context is provided to the user for the current action.
        /// </summary>
        private void InitializeWindowTitle(Purpose purpose)
        {
            switch(purpose)
            {
                case Purpose.Export:
                    Title = "Export Extensions";
                    break;

                case Purpose.InstallForSolution:
                    Title = Vsix.Name;
                    break;

                case Purpose.Import:
                    Title = "Import Extensions";
                    break;

            }
        }

        /// <summary>
        /// Alters the chrome (title bar, close/minimize/maximize buttons etc.) to correspond to a dialog box style window.
        /// </summary>
        private void InitializeWindowChrome()
        {
            this.StyleWindowAsDialogBox();
        }

        public List<Extension> SelectedExtension { get; private set; }

        public bool InstallSystemWide { get; private set; }

        private void ImportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hasCategory = false;
            IEnumerable<Extension> sortedList = _extensions;

            if (_purpose == Purpose.Import)
            {
                sortedList = _extensions.OrderByDescending(x => x.Selected).ThenBy(x => x.Name);
            }

            if (_extensions.FirstOrDefault()?.Selected == true)
            {
                var label = new Label {
                    Content = "Extensions",
                    Margin = new Thickness(0, 0, 0, 5),
                    FontWeight = FontWeights.Bold
                };

                list.Children.Add(label);
            }

            foreach (Extension ext in sortedList)
            {
                var cb = new CheckBox {
                    Content = ext.Name,
                    IsChecked = ext.Selected,
                    CommandParameter = ext.ID,
                    Margin = new Thickness(10, 0, 0, 0),
                };

                if (_purpose == Purpose.Import && !ext.Selected)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedExtension = new List<Extension>();

            foreach (CheckBox cb in list.Children.OfType<CheckBox>())
            {
                if (cb.IsChecked == true && cb.IsEnabled == true)
                {
                    SelectedExtension.Add(_extensions.First(ext => ext.ID == (string)cb.CommandParameter));
                }
            }

            InstallSystemWide = chkInstallSystemWide.IsChecked ?? false;
            DialogResult = true;
            Close();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedExtension = new List<Extension>();

            foreach (CheckBox cb in list.Children.OfType<CheckBox>())
            {
                cb.IsChecked = true;
            }
        }

        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedExtension = new List<Extension>();

            foreach (CheckBox cb in list.Children.OfType<CheckBox>())
            {
                cb.IsChecked = false;
            }
        }

        public static ImportWindow Open(IEnumerable<Extension> extensions, Purpose purpose, string msg = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            var dialog = new ImportWindow(extensions, purpose, msg);
            var hwnd = new IntPtr(dte.MainWindow.HWnd);

            var window = (Window)HwndSource.FromHwnd(hwnd).RootVisual;

            dialog.Owner = window;
            dialog.ShowDialog();

            return dialog;
        }

        private void SelectDeselectAll_Checked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAll();
        }

        private void SelectDeselectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAll();
        }

        /// <summary>
        /// Called to implement the functionality of the Select/Deselect All check box.
        /// </summary>
        private void SelectDeselectAll()
        {
            // If check boxes are grayed out, always clear their check boxes
            // since they are not applicable to the task at hand.
            if (list.Children.OfType<CheckBox>().Any(cb => !cb.IsEnabled))
            {
                foreach (var cb in list.Children.OfType<CheckBox>().Where(cb => !cb.IsEnabled))
                    cb.IsChecked = false;
                return;
            }

            // Only let the Select/Deselect All check box work on those check boxes
            // that aren't grayed out.
            if (!list.Children.OfType<CheckBox>().Any(cb => cb.IsEnabled))
                return;

            foreach (var cb in list.Children.OfType<CheckBox>().Where(cb => cb.IsEnabled))
                cb.IsChecked = chkSelectDeselectAll.IsChecked;
        }
    }
}
