using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio;
using ExtensionManager.VisualStudio.Extensions;
using Task = System.Threading.Tasks.Task;

namespace ExtensionManager.Installation;

internal sealed class ExtensionInstaller : IExtensionInstaller
{
    public async Task InstallAsync(IReadOnlyCollection<IVSExtension> extensions, bool installSystemWide, IProgress<ProgressStep<InstallStep>> uiProgress, CancellationToken cancellationToken)
    {
        if (extensions.Count == 0)
            return;

        cancellationToken.ThrowIfCancellationRequested();

        uiProgress.Report(null, InstallStep.DownloadData);

        extensions = await VSFacade.Extensions.GetGalleryExtensionsAsync(extensions.Select(x => x.Id)).ConfigureAwait(false);

        try
        {
            IReadOnlyList<ExtensionDownloader> downloaders;

            using (var httpMessageHandler = new HttpClientHandler())
            {
                httpMessageHandler.MaxConnectionsPerServer = 100;

                downloaders = CreateDownloaders(httpMessageHandler, extensions, uiProgress, cancellationToken);

                if (downloaders.Count == 0)
                    return;

                var success = await RunDownloadsAsync(downloaders, cancellationToken).ConfigureAwait(false);

                if (!success)
                    return;
            }

            await RunInstallationAsync(downloaders, installSystemWide, uiProgress, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Only ClearAsync does not seem to work
            await VSFacade.StatusBar.ShowProgressAsync("", 1, 1);
            await VSFacade.StatusBar.ShowMessageAsync("");
            await VSFacade.StatusBar.ClearAsync();
        }
    }

    private IReadOnlyList<ExtensionDownloader> CreateDownloaders(HttpMessageHandler httpMessageHandler, IReadOnlyCollection<IVSExtension> extensions, IProgress<ProgressStep<InstallStep>> uiProgress, CancellationToken cancellationToken)
    {
        var progress = new DownloadProgres(uiProgress, extensions.Count);
        var entries = new List<ExtensionDownloader>();

        var targetFolder = PrepareDownloadTargetFolder(cancellationToken);

        foreach (var extension in extensions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (extension.DownloadUrl is null or { Length: 0 })
                continue;

            if (!Uri.TryCreate(extension.DownloadUrl, UriKind.Absolute, out var downloadUri))
                continue;

            var targetFilePath = GetDownloadTargetFilePath(targetFolder, extension.DownloadUrl);

            entries.Add(new ExtensionDownloader(httpMessageHandler, progress, downloadUri, targetFilePath, extension, cancellationToken));
        }

        return entries;
    }
    private static string PrepareDownloadTargetFolder(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

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

    private async Task<bool> RunDownloadsAsync(IReadOnlyList<ExtensionDownloader> entries, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await VSFacade.StatusBar.ShowMessageAsync($"Downloading {entries.Count} extensions ...").ConfigureAwait(false);

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            entry.BeginDownload();
        }

        cancellationToken.ThrowIfCancellationRequested();

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

    private async Task RunInstallationAsync(IReadOnlyCollection<ExtensionDownloader> entries, bool installSystemWide, IProgress<ProgressStep<InstallStep>> uiProgress, CancellationToken cancellationToken)
    {
        var vsixFiles = entries
            .Where(x => x.DownloadException is null)
            .Select(x => x.TargetFilePath)
            .ToArray();

        if (vsixFiles.Length == 0)
            return;

        cancellationToken.ThrowIfCancellationRequested();

        uiProgress.Report(null, InstallStep.RunInstallation);

        await VSFacade.StatusBar.ShowMessageAsync($"Extensions downloaded. Starting VSIX Installer ...");
        await VSFacade.Extensions.StartInstallerAsync(vsixFiles, installSystemWide);
    }
}
