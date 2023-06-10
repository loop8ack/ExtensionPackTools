using System.Threading;
using System.Threading.Tasks;

namespace ExtensionManager;

public interface IFeature
{
    Task ExecuteAsync();
}
