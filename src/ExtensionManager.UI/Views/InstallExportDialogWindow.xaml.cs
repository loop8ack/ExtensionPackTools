using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

using ExtensionManager.UI.ViewModels;
using ExtensionManager.UI.Win32;

namespace ExtensionManager.UI.Views;

internal partial class InstallExportDialogWindow : Window
{
    public InstallExportDialogWindow()
    {
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        DataContextChanged += OnDataContextChanged;

        InitializeComponent();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is InstallExportDialogViewModel vm)
        {
            vm.CloseRequested -= OnCloseRequested;
            vm.CloseRequested += OnCloseRequested;
        }
    }

    private void OnCloseRequested(object sender, EventArgs e)
    {
        if (sender is not InstallExportDialogViewModel vm)
            return;

        if (!ReferenceEquals(vm, DataContext))
        {
            vm.CloseRequested -= OnCloseRequested;
            return;
        }

        Close();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        this.StyleWindowAsDialogBox();
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        if (DataContext is InstallExportDialogViewModel vm)
            vm.OnClosed();
    }

    private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true,
        });
    }
}
