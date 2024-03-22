using Microsoft.VisualStudio.Shell;

using Task = System.Threading.Tasks.Task;

namespace ExtensionManager.VisualStudio.Threads;

internal sealed class VSThreads : IVSThreads
{
    public bool CheckUIThreadAccess()
        => ThreadHelper.CheckAccess();

    public async Task RunOnUIThreadAsync(Action syncMethod)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        syncMethod();
    }

    public async Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> syncMethod)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        return syncMethod();
    }
}
