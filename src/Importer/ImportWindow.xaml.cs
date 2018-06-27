using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.ExtensionManager;
using static ExtensionPackTools.Manifest;

namespace ExtensionPackTools.Importer
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private readonly IEnumerable<Extension> _extensions;
        private readonly IVsExtensionManager _manager;
        private readonly Purpose _purpose;

        public ImportWindow(IEnumerable<Extension> extensions, IVsExtensionManager manager, Purpose purpose)
        {
            _extensions = extensions;
            _manager = manager;
            _purpose = purpose;
            Loaded += ImportWindow_Loaded;
            InitializeComponent();
            Title = Vsix.Name;
        }

        public List<string> SelectedExtensionIds { get; private set; }

        private void ImportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Extension ext in _extensions)
            {
                var cb = new CheckBox
                {
                    Content = ext.Name,
                    IsChecked = true,
                    CommandParameter = ext.ID,
                };

                if (_purpose == Purpose.Import)
                {
                    cb.IsEnabled = !_manager.TryGetInstalledExtension(ext.ID, out IInstalledExtension installed);
                }

                list.Children.Add(cb);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedExtensionIds = new List<string>();

            foreach (CheckBox cb in list.Children)
            {
                if (cb.IsChecked == true && cb.IsEnabled == true)
                {
                    SelectedExtensionIds.Add((string)cb.CommandParameter);
                }
            }

            DialogResult = true;
            Close();
        }
    }

    public enum Purpose
    {
        Import,
        Export
    }
}
