using System.Threading.Tasks;

namespace ExtensionManager;
public interface IFeatureExecutor
{
    Task ExecuteAsync<TFeature>() where TFeature : class, IFeature;
}