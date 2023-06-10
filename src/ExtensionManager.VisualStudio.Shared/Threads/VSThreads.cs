using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager.VisualStudio.Threads;

internal sealed class VSThreads : IVSThreads
{
    public bool CheckUIThreadAccess()
        => ThreadHelper.CheckAccess();

    public async Task<TResult> RunOnUIThreadAsync<TResult>(Func<TResult> syncMethod)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        return syncMethod();
    }
}
