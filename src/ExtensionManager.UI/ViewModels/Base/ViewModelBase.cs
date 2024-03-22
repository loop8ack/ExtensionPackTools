using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ExtensionManager.UI.ViewModels.Base;

internal abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetValue<T>([NotNullIfNotNull(nameof(value))] ref T field, T value, [CallerMemberName] string propertyName = null!)
    {
        return SetValue(ref field, value, onChanged: null, propertyName);
    }
    protected bool SetValue<T>([NotNullIfNotNull(nameof(value))] ref T field, T value, Action? onChanged, [CallerMemberName] string propertyName = null!)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            field = value;

            NotifyPropertyChanged(propertyName);

            onChanged?.Invoke();

            return true;
        }

        return false;
    }

    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
