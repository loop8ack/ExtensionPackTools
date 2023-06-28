using System;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest;

namespace ExtensionManager.UI.Worker;

public interface IExportWorker
{
    Task ExportAsync(IManifest manifest, IProgress<ProgressStep<ExportStep>> progress, CancellationToken cancellationToken);
}
