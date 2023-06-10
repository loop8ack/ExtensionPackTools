using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using ExtensionManager.VisualStudio.Models;

using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Setup.Configuration;

namespace ExtensionManager.VisualStudio.Extensions;

internal sealed class VSExtensions : IVSExtensions
{
    public async Task<IReadOnlyCollection<IVSExtension>> GetGalleryExtensionsAsync(IEnumerable<string> extensionIds)
    {
        var repository = await VS.GetRequiredServiceAsync<SVsExtensionRepository, IVsExtensionRepository>();

        var extensionIdsList = extensionIds as List<string> ?? extensionIds.ToList();

        return repository
            .GetVSGalleryExtensions<GalleryExtension>(extensionIdsList, 1033, false)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<string>> GetInstalledExtensionIdsAsync()
    {
        var manager = await VS.GetRequiredServiceAsync<SVsExtensionManager, IVsExtensionManager>();

        return manager.GetInstalledExtensions()
            .Where(i => !i.Header.SystemComponent && !i.IsPackComponent)
            .Select(i => i.Header.Identifier)
            .ToArray();
    }

    public async Task StartInstallerAsync(IEnumerable<string> vsixFiles, bool systemWide)
    {
        var rootSuffix = await VS.Shell.TryGetCommandLineArgumentAsync("rootsuffix").ConfigureAwait(false);

        var arguments = $"{string.Join(" ", vsixFiles)} /instanceIds:{GetInstallationId()}";

        if (systemWide)
            arguments += $" /admin";

        if (!string.IsNullOrEmpty(rootSuffix))
            arguments += $" /rootSuffix:{rootSuffix}";

        Process.Start(new ProcessStartInfo
        {
            FileName = GetVsixInstallerFilePath(),
            Arguments = arguments,
            UseShellExecute = false,
        });

        static string GetInstallationId()
        {
            return ((ISetupConfiguration)new SetupConfiguration())
                .GetInstanceForCurrentProcess()
                .GetInstanceId();
        }

        static string GetVsixInstallerFilePath()
        {
            var process = Process.GetCurrentProcess();
            var dir = Path.GetDirectoryName(process.MainModule.FileName);
            return Path.Combine(dir, "VSIXInstaller.exe");
        }
    }
}
