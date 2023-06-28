using System;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio.MessageBox;

using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager;

internal sealed class FeatureExecutor : IFeatureExecutor
{
    private readonly IServiceProvider _services;

    public FeatureExecutor(IServiceProvider services)
    {
        _services = services;
    }

    public async Task ExecuteAsync<TFeature>()
        where TFeature : class, IFeature
    {
        try
        {
            await ActivatorUtilities
                .GetServiceOrCreateInstance<TFeature>(_services)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _services
                .GetRequiredService<IVSMessageBox>()
                .ShowErrorAsync(ex.Message);
        }
    }
}
