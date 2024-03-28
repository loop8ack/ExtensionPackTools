using System.Threading.Tasks;

using VS = Community.VisualStudio.Toolkit.VS;

#nullable enable

namespace ExtensionManager.VisualStudio.StatusBar;

internal sealed class VSStatusBar : IVSStatusBar
{
    public Task ClearAsync() => VS.StatusBar.ClearAsync();
    public Task ShowMessageAsync(string text) => VS.StatusBar.ShowMessageAsync(text);
    public Task ShowProgressAsync(string text, int currentStep, int numberOfSteps) => VS.StatusBar.ShowProgressAsync(text, currentStep, numberOfSteps);
}
