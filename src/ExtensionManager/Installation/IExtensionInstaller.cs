using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Installation;

public interface IExtensionInstaller
{
    Task InstallAsync(IReadOnlyCollection<IVSExtension> extensions, bool installSystemWide, IProgress<ProgressStep<InstallStep>> uiProgress, CancellationToken cancellationToken);
}
