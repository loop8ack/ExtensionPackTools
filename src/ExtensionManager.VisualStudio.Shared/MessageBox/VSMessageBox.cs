using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio;

namespace ExtensionManager.VisualStudio.MessageBox;

internal sealed class VSMessageBox : IVSMessageBox
{
    public Task ShowErrorAsync(string line1, string line2 = "") => VS.MessageBox.ShowErrorAsync(line1, line2);

    public async Task<bool> ShowWarningAsync(string line1, string line2 = "")
    {
        var result = await VS.MessageBox.ShowWarningAsync(line1, line2).ConfigureAwait(false);

        return result == VSConstants.MessageBoxResult.IDOK;
    }
}
