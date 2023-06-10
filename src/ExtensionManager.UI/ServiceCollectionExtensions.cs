using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.UI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDialogService(this IServiceCollection services)
    {
        services.AddTransient<IDialogService, DialogService>();

        return services;
    }
}
