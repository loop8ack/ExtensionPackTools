using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using ExtensionManager.UI.ViewModels.Base;

namespace ExtensionManager.UI.ViewModels;

internal class InstallExportDialogViewModel : ViewModelBase
{
    private bool _systemWide;

    public event EventHandler<bool?>? CloseRequested;

    public ObservableCollection<ExtensionViewModel> Extensions { get; } = new();

    public bool HasAllSelected
    {
        get
        {
            var hasAny = false;

            foreach (var ext in Extensions)
            {
                if (!ext.CanBeSelected)
                    continue;

                if (!ext.IsSelected)
                    return false;

                hasAny = true;
            }

            return hasAny;
        }
        set
        {
            foreach (var item in Extensions)
            {
                if (item.CanBeSelected)
                    item.IsSelected = value;
            }
        }
    }

    public bool HasAnySelected => Extensions.Any(x => x.IsSelected);
    public IEnumerable<ExtensionViewModel> SelectedExtensions => Extensions.Where(x => x.IsSelected);

    public bool SystemWide
    {
        get => _systemWide;
        set => SetValue(ref _systemWide, value);
    }

    public InstallExportDialogType DialogType { get; }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    public InstallExportDialogViewModel(InstallExportDialogType dialogType)
    {
        DialogType = dialogType;

        OkCommand = new DelegateCommand(() => RequestClose(true), () => HasAnySelected);
        CancelCommand = new DelegateCommand(() => RequestClose(false));

        Extensions.CollectionChanged += OnExtensionsCollectionChanged;
    }

    private void RequestClose(bool? result)
        => CloseRequested?.Invoke(this, result);

    private void OnExtensionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<ExtensionViewModel>())
                item.PropertyChanged -= OnExtensionPropertyChanged;
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<ExtensionViewModel>())
                item.PropertyChanged += OnExtensionPropertyChanged;
        }
    }

    private void OnExtensionPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(ExtensionViewModel.IsSelected):
                NotifyPropertyChanged(nameof(HasAllSelected));
                NotifyPropertyChanged(nameof(HasAnySelected));
                NotifyPropertyChanged(nameof(SelectedExtensions));
                break;

            case nameof(ExtensionViewModel.CanBeSelected):
                NotifyPropertyChanged(nameof(HasAllSelected));
                break;
        }
    }
}
