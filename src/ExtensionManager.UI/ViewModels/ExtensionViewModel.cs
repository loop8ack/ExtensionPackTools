using ExtensionManager.UI.ViewModels.Base;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.UI.ViewModels;

internal class ExtensionViewModel : ViewModelBase
{
    private bool _canBeSelected = true;
    private bool _isSelected;
    private string? _group;

    public IVSExtension Model { get; }
    public string VsixID => Model.Id;
    public string? Name => Model.Name;
    public string? MoreInfoURL => Model.MoreInfoURL;

    public bool CanBeSelected
    {
        get => _canBeSelected;
        set
        {
            if (SetValue(ref _canBeSelected, value))
                NotifyPropertyChanged(nameof(IsSelected));
        }
    }
    public bool IsSelected
    {
        get => CanBeSelected && _isSelected;
        set => SetValue(ref _isSelected, value);
    }
    public string? Group
    {
        get => _group;
        set => SetValue(ref _group, value);
    }

    public ExtensionViewModel(IVSExtension model)
    {
        Model = model;
    }
}
