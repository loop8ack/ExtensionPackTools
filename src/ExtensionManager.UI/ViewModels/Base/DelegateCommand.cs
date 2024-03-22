using System.Windows.Input;

namespace ExtensionManager.UI.ViewModels.Base;

internal sealed class DelegateCommand : ICommand
{
    private readonly Func<bool>? _canExecute;
    private readonly Action _execute;

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public DelegateCommand(Action execute)
    {
        _execute = execute;
    }
    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
        _canExecute = canExecute;
        _execute = execute;
    }

    public bool CanExecute(object parameter)
        => _canExecute is null || _canExecute();

    public void Execute(object parameter)
        => _execute();
}
