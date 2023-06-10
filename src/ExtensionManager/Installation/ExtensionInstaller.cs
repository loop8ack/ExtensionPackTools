using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;

using Task = System.Threading.Tasks.Task;

namespace ExtensionManager.Installation;

internal sealed class ExtensionInstaller : IExtensionInstaller
{
    public async Task InstallAsync(IReadOnlyCollection<IVSExtension> extensions, bool installSystemWide)
    {
        if (extensions.Count == 0)
            return;

        extensions = await VSFacade.Extensions.GetGalleryExtensionsAsync(extensions.Select(x => x.Id)).ConfigureAwait(false);

        try
        {
            IReadOnlyList<ExtensionDownloader> downloaders;

            using (var httpMessageHandler = new HttpClientHandler())
            {
                httpMessageHandler.MaxConnectionsPerServer = 100;

                downloaders = CreateDownloaders(httpMessageHandler, extensions);

                if (downloaders.Count == 0)
                    return;

                var success = await RunDownloadsAsync(downloaders).ConfigureAwait(false);

                if (!success)
                    return;
            }

            await RunInstallationAsync(downloaders, installSystemWide).ConfigureAwait(false);
        }
        finally
        {
            // ClearAsync does not seem to work
            await VSFacade.StatusBar.ShowMessageAsync("");
            await VSFacade.StatusBar.ClearAsync();
        }
    }

    private IReadOnlyList<ExtensionDownloader> CreateDownloaders(HttpMessageHandler httpMessageHandler, IReadOnlyCollection<IVSExtension> extensions)
    {
        var progress = new DownloadProgres(extensions.Count);
        var entries = new List<ExtensionDownloader>();

        var targetFolder = PrepareDownloadTargetFolder();

        foreach (var extension in extensions)
        {
            if (extension.DownloadUrl is null or { Length: 0 })
                continue;

            if (!Uri.TryCreate(extension.DownloadUrl, UriKind.Absolute, out var downloadUri))
                continue;

            var targetFilePath = GetDownloadTargetFilePath(targetFolder, extension.DownloadUrl);

            entries.Add(new ExtensionDownloader(httpMessageHandler, progress, downloadUri, targetFilePath, extension));
        }

        return entries;
    }
    private static string PrepareDownloadTargetFolder()
    {
        var tempFolder = Path.Combine(Path.GetTempPath(), nameof(ExtensionManager));

        if (Directory.Exists(tempFolder))
            Directory.Delete(tempFolder, true);

        Directory.CreateDirectory(tempFolder);

        return tempFolder;
    }
    private string GetDownloadTargetFilePath(string targetFolder, string downloadUrl)
    {
        return Path.Combine(targetFolder, CreateMD5(downloadUrl) + ".vsix");

        static string CreateMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }

    private async Task<bool> RunDownloadsAsync(IReadOnlyList<ExtensionDownloader> entries)
    {
        await VSFacade.StatusBar.ShowMessageAsync($"Downloading {entries.Count} extensions ...").ConfigureAwait(false);

        foreach (var entry in entries)
            entry.BeginDownload();

        await Task.WhenAll(entries.Select(x => x.DownloadTask)).ConfigureAwait(false);

        var failedCount = entries.Count(x => x.DownloadException is not null);

        if (failedCount > 0)
        {
            return await VSFacade.MessageBox.ShowWarningAsync(
                $"The download of {failedCount} extensions failed.",
                "Continue?");
        }

        return true;
    }

    private async Task RunInstallationAsync(IReadOnlyCollection<ExtensionDownloader> entries, bool installSystemWide)
    {
        var vsixFiles = entries
            .Where(x => x.DownloadException is null)
            .Select(x => x.TargetFilePath)
            .ToArray();

        if (vsixFiles.Length == 0)
            return;

        await VSFacade.StatusBar.ShowMessageAsync($"Extensions downloaded. Starting VSIX Installer ...");
        await VSFacade.Extensions.StartInstallerAsync(vsixFiles, installSystemWide);
    }
}
