using Microsoft.Extensions.DependencyInjection;
namespace ExtensionManager.VisualStudio;

public interface IVSServicesRegistrar
{
    void AddServices(IServiceCollection services);
}
