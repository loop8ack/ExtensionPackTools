using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;

using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace ExtensionManager
{
    internal class ExtensionInstaller
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IVisualStudioService _vsService;
        private int _currentCount;
        private int _entriesCount;

        public ExtensionInstaller(IServiceProvider serviceProvider, IVisualStudioService vsService)
        {
            _serviceProvider = serviceProvider;
            _vsService = vsService;
        }

        public void Install(IEnumerable<Extension> extension, bool installSystemWide)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var toInstall = extension.Select(ext => ext.ID).ToList();

            IEnumerable<IGalleryEntry> marketplaceEntries = _vsService.GetGalleryEntries(toInstall).Where(x => x.DownloadUrl != null);
            var tempDir = PrepareTempDir();

            var dte = _serviceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            dte.StatusBar.Text = "Downloading extensions...";

            HasRootSuffix(out var rootSuffix);
            ThreadHelper.JoinableTaskFactory.Run(() => DownloadExtensionsAsync(installSystemWide, marketplaceEntries, tempDir, dte, rootSuffix));
        }

        private async Task DownloadExtensionsAsync(bool installSystemWide, IEnumerable<IGalleryEntry> marketplaceEntries, string tempDir, DTE dte, string rootSuffix)
        {
            ServicePointManager.DefaultConnectionLimit = 100;
            _entriesCount = marketplaceEntries.Count();
            _currentCount = 0;
            await DownloadExtensionAsync(marketplaceEntries, tempDir, dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            dte.StatusBar.Text = "Extensions downloaded. Starting VSIX Installer...";
            InvokeVsixInstaller(tempDir, rootSuffix, installSystemWide);
        }

        private static string PrepareTempDir()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), nameof(ExtensionManager));

            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        private void InvokeVsixInstaller(string tempDir, string rootSuffix, bool installSystemWide)
        {
            var process = Process.GetCurrentProcess();
            var dir = Path.GetDirectoryName(process.MainModule.FileName);
            var exe = Path.Combine(dir, "VSIXInstaller.exe");
            var configuration = new SetupConfiguration() as ISetupConfiguration;
            var adminSwitch = installSystemWide ? "/admin" : string.Empty;
            ISetupInstance instance = configuration.GetInstanceForCurrentProcess();
            IEnumerable<string> vsixFiles = Directory.EnumerateFiles(tempDir, "*.vsix").Select(f => Path.GetFileName(f));

            var start = new ProcessStartInfo {
                FileName = exe,
                Arguments = $"{string.Join(" ", vsixFiles)} /instanceIds:{instance.GetInstanceId()} {adminSwitch}",
                WorkingDirectory = tempDir,
                UseShellExecute = false,
            };

            if (!string.IsNullOrEmpty(rootSuffix))
            {
                start.Arguments += $" /rootSuffix:{rootSuffix}";
            }

            Process.Start(start);
        }

        private Task DownloadExtensionAsync(IEnumerable<IGalleryEntry> entries, string dir, DTE dte)
        {
            return Task.WhenAll(entries.Select(entry => Task.Run(() => DownloadExtensionFileAsync(entry, dir, dte))).ToArray());
        }

        private async Task DownloadExtensionFileAsync(IGalleryEntry entry, string dir, DTE dte)
        {
            var localPath = Path.Combine(dir, CreateMD5(entry.DownloadUrl) + ".vsix");

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(entry.DownloadUrl, localPath);
            }

            await UpdateProgressAsync(dte);
        }

        private async Task UpdateProgressAsync(DTE dte)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            dte.StatusBar.Text = $"Downloaded {Interlocked.Increment(ref _currentCount)} of {_entriesCount} extensions...";
            await TaskScheduler.Default;
        }

        private static string CreateMD5(string input)
        {
            try
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
        public static bool HasRootSuffix(out string rootSuffix)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            rootSuffix = null;

            if (ServiceProvider.GlobalProvider.GetService(typeof(SVsAppCommandLine)) is IVsAppCommandLine appCommandLine)
            {
                if (ErrorHandler.Succeeded(appCommandLine.GetOption("rootsuffix", out var hasRootSuffix, out rootSuffix)))
                {
                    return hasRootSuffix != 0;
                }
            }

            return false;
        }
    }
}
