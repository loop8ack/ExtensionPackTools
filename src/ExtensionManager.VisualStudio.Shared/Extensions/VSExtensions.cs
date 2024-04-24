using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using ExtensionManager.VisualStudio.Adapter.Extensions;

using Microsoft.VisualStudio.Setup.Configuration;

#nullable enable

namespace ExtensionManager.VisualStudio.Extensions;

internal sealed class VSExtensions : IVSExtensions
{
    private readonly IVSExtensionRepositoryAdapter _repositoryAdapter;
    private readonly IVSExtensionManagerAdapter _managerAdapter;

    public VSExtensions(IVSExtensionRepositoryAdapter repositoryAdapter, IVSExtensionManagerAdapter managerAdapter)
    {
        _repositoryAdapter = repositoryAdapter;
        _managerAdapter = managerAdapter;
    }

    public async Task<IReadOnlyCollection<IVSExtension>> GetGalleryExtensionsAsync(IEnumerable<string> extensionIds)
    {
        var extensionIdsList = extensionIds as List<string> ?? extensionIds.ToList();

        return await _repositoryAdapter.GetVSGalleryExtensionsAsync(extensionIdsList, 1033, false);
    }

    public async Task<IReadOnlyCollection<IVSExtension>> GetInstalledExtensionsAsync()
    {
        var installedExtensions = await _managerAdapter.GetInstalledExtensionsAsync();

        var extensionIds = installedExtensions
            .Where(i => !i.IsSystemComponent)
            .Where(i => !i.IsPackComponent)
            .Select(i => i.Identifier)
            .ToList();

        return await GetGalleryExtensionsAsync(extensionIds);
    }

    public async Task StartInstallerAsync(IEnumerable<string> vsixFiles, bool systemWide)
    {
        var rootSuffix = await VS.Shell.TryGetCommandLineArgumentAsync("rootsuffix").ConfigureAwait(false);

        vsixFiles = vsixFiles.Select(x => $"{x}");

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
