using Microsoft.Extensions.DependencyInjection;

namespace ExtensionManager.VisualStudio;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureVSFacade(this IServiceCollection services, IVSServiceFactory serviceFactory)
    {
        services.AddSingleton<IVSFacade>(new VSFacade(serviceFactory));

        services.AddTransient(s => s.GetRequiredService<IVSFacade>().Themes);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().Threads);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().Solutions);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().StatusBar);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().Documents);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().MessageBox);
        services.AddTransient(s => s.GetRequiredService<IVSFacade>().Extensions);

        return services;
    }
}
