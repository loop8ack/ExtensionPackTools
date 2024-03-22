using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

using ExtensionManager.UI.ViewModels.Base;
using ExtensionManager.UI.Worker;

namespace ExtensionManager.UI.ViewModels;

internal abstract class InstallExportDialogViewModel<TStep> : InstallExportDialogViewModel, IProgress<ProgressStep<TStep>>
{
    private CancellationTokenSource? _cts;

    protected InstallExportDialogViewModel(InstallExportDialogType dialogType)
        : base(dialogType)
    {
    }

    protected override bool CanOk()
    {
        return base.CanOk()
            && !IsRunning;
    }
    protected override async void OnOk()
    {
        await RunWorkAsync();

        RequestClose();
    }
    private async Task RunWorkAsync()
    {
        using var cts = new CancellationTokenSource();

        Interlocked.Exchange(ref _cts, cts);

        IsRunning = true;

        try
        {
            await DoWorkAsync(this, cts.Token);
        }
        catch (OperationCanceledException)
            when (cts.IsCancellationRequested)
        {
        }
        finally
        {
            IsRunning = false;

            Interlocked.CompareExchange(ref _cts, null, cts);
        }
    }

    protected override void OnCancel()
    {
        _cts?.Cancel();

        RequestClose();
    }

    public override void OnClosed()
    {
        var cts = Interlocked.Exchange(ref _cts, null);

        if (cts is not null)
        {
            cts.Cancel();
            cts.Dispose();
        }
    }

    protected abstract Task DoWorkAsync(IProgress<ProgressStep<TStep>> progress, CancellationToken cancellationToken);
    protected abstract string? GetStepMessage(TStep step);

    void IProgress<ProgressStep<TStep>>.Report(ProgressStep<TStep> value)
    {
        ProgressText = GetStepMessage(value.Step);
        ProgressPercentage = value.Percentage;
    }
}

internal abstract class InstallExportDialogViewModel : ViewModelBase
{
    private bool _systemWide;
    private bool _isRunning;
    private string? _progressText;
    private float? _progressPercentage;

    public event EventHandler? CloseRequested;

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

    public bool IsRunning
    {
        get => _isRunning;
        protected set => SetValue(ref _isRunning, value);
    }
    public string? ProgressText
    {
        get => _progressText;
        protected set => SetValue(ref _progressText, value);
    }
    public float? ProgressPercentage
    {
        get => _progressPercentage;
        protected set => SetValue(ref _progressPercentage, value);
    }

    public InstallExportDialogType DialogType { get; }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    protected InstallExportDialogViewModel(InstallExportDialogType dialogType)
    {
        DialogType = dialogType;

        OkCommand = new DelegateCommand(OnOk, CanOk);
        CancelCommand = new DelegateCommand(OnCancel, CanCancel);

        Extensions.CollectionChanged += OnExtensionsCollectionChanged;
    }

    protected virtual bool CanOk() => HasAnySelected;
    protected abstract void OnOk();

    protected virtual bool CanCancel() => true;
    protected abstract void OnCancel();

    public virtual void OnClosed() { }

    protected void RequestClose()
        => CloseRequested?.Invoke(this, EventArgs.Empty);

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
