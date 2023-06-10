using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Installation;

public interface IExtensionInstaller
{
    Task InstallAsync(IReadOnlyCollection<IVSExtension> extension, bool installSystemWide);
}
