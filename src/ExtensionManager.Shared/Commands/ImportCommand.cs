using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using ExtensionManager.Core.Models.Interfaces;
using ExtensionManager.Core.Services.Interfaces;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;

namespace ExtensionManager
{
    /// <summary>
    /// Provides functionality for the <c>Import Extensions...</c> command on the menu.
    /// </summary>
    internal sealed class ImportCommand : CommandBase

    {
        /// <summary>
        /// Counter of how many extensions have been download thus far.
        /// </summary>
        /// <remarks>
        /// This should be left as a field, not a property, since we need to pass
        /// it by reference.
        /// </remarks>
        private int _currentCount;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.ImportCommand" /> and returns a reference
        /// to it.
        /// </summary>
        /// <param name="package">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsPackage" /> interface.
        /// </param>
        /// <param name="commandService">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:System.ComponentModel.Design.IMenuCommandService" /> interface.
        /// </param>
        /// <param name="extensionService">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </param>
        /// null
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the any of the
        /// required parameters, <paramref name="package" />,
        /// <paramref name="commandService" />, or <paramref name="extensionService" />,
        /// are passed a <see langword="null" /> value.
        /// </exception>
        private ImportCommand(IVsPackage package,
            IMenuCommandService commandService,
            IExtensionService extensionService) : base(
            package, commandService, extensionService
        )
        {
            /*
             * Call the base class to perform the initialization
             * because similar steps are necessary for all
             * commands.
             */

            AddCommandToVisualStudioMenus(
                Execute, PackageGuids.guidExportPackageCmdSet,
                PackageIds.ImportCmd,
                /* supported = */ false
            );
        }

        /// <summary>
        /// Gets a reference to the sole instance of the
        /// <see cref="T:ExtensionManager.ImportCommand" /> object that is being used to
        /// implement the functionality of importing extensions.
        /// </summary>
        public static ImportCommand Instance { get; private set; }

        /// <summary>
        /// Gets or sets the total count of how many extensions need to be downloaded.
        /// </summary>
        private int EntriesCount { get; set; }

        /// <summary>
        /// Creates and initializes a new instance of
        /// <see cref="T:ExtensionManager.ImportCommand" /> and sets the
        /// <see cref="P:ExtensionManager.ImportCommand.Instance" /> property to a
        /// reference to it.
        /// </summary>
        /// <param name="package">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsPackage" /> interface.
        /// </param>
        /// <param name="commandService">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:System.ComponentModel.Design.IMenuCommandService" /> interface.
        /// </param>
        /// <param name="extensionService">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </param>
        /// null
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the any of the
        /// required parameters, <paramref name="package" />,
        /// <paramref name="commandService" />, or <paramref name="extensionService" />,
        /// are passed a <see langword="null" /> value.
        /// </exception>
        public static void Initialize(IVsPackage package,
            IMenuCommandService commandService,
            IExtensionService extensionService)
        {
            Instance = new ImportCommand(
                package, commandService, extensionService
            );
        }

        /// <summary>
        /// Supplies code that is to be executed when the user chooses this command from
        /// menus or toolbars.
        /// </summary>
        /// <param name="sender">Reference to the sender of the event.</param>
        /// <param name="e">
        /// A <see cref="T:System.EventArgs" /> that contains the event
        /// data.
        /// </param>
        public override void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var filePath = Get.ImportFilePath();
            if (string.IsNullOrWhiteSpace(filePath)) return;

            var manifest = Manifest.FromFile(filePath);
            manifest.MarkSelected(_extensionService.GetInstalledExtensions());

            var dialog = ImportWindow.Open(manifest.Extensions, Purpose.Import);

            if (dialog.DialogResult != true ||
                !dialog.SelectedExtensions.Any()) return;

            var installSystemWide = dialog.InstallSystemWide;
            var toInstall = dialog.SelectedExtensions.Select(ext => ext.ID)
                                  .ToList();

            if (!(ServiceProvider.GetService(typeof(SVsExtensionRepository)) is
                    IVsExtensionRepository repository)) return;
            Assumes.Present(repository);

            var marketplaceEntries = repository
                                     .GetVSGalleryExtensions<GalleryEntry>(
                                         toInstall, 1033, false
                                     )
                                     .Where(x => x.DownloadUrl != null);
            var tempDir = Get.DefaultTempFolderPath;
            if (!Replace.Folder(tempDir)) return;

            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            dte.StatusBar.Text = "Downloading extensions...";

            HasRootSuffix(out var rootSuffix);
            ThreadHelper.JoinableTaskFactory.Run(
                () => DownloadExtensionsAsync(
                    installSystemWide, marketplaceEntries, tempDir, dte,
                    rootSuffix
                )
            );
        }

        /// <summary>
        /// Called to carry out the download operation for the extensions that the user
        /// wants to import.
        /// </summary>
        /// <param name="installSystemWide">
        /// (Required.) A <see cref="T:System.Boolean" />
        /// value that specifies whether or not to install extensions for all the users on
        /// the machine.
        /// <para />
        /// Set to <see langword="true" /> to install for all users;
        /// <see langword="false" /> otherwise.
        /// </param>
        /// <param name="marketplaceEntries">
        /// (Required.) Collection of instances of objects
        /// that implement the
        /// <see cref="T:ExtensionManager.Core.Models.Interfaces.IGalleryEntry" />
        /// interface, each of which contains the information necessary to obtain the
        /// extension's <c>.vsix</c> file from the Visual Studio Marketplace.
        /// <para />
        /// This collection should contain more than zero elements.
        /// </param>
        /// <param name="tempDir">
        /// (Required.) String containing the fully-qualified
        /// pathname of a folder on the disk into which extensions will be downloaded.
        /// <para />
        /// The folder is created if it does not already exist.
        /// </param>
        /// <param name="dte">(Required.) </param>
        /// <param name="rootSuffix"></param>
        /// <returns></returns>
        private async Task DownloadExtensionsAsync(bool installSystemWide,
            IEnumerable<IGalleryEntry> marketplaceEntries, string tempDir,
            DTE dte, string rootSuffix)
        {
            // If a blank string is passed for the argument of the
            // tempDir parameter, then specify, as a default, a folder
            // in the Temp directory that is named after a GUID.
            if (string.IsNullOrWhiteSpace(tempDir))
                tempDir = Get.DefaultTempFolderPath;

            /* astrohart - I know we did this in the caller of this method, but
               what if the code changes in the future?  This way, we can be 100% sure
               the download folder exists.*/
            if (!Replace.Folder(tempDir)) return;

            ServicePointManager.DefaultConnectionLimit = 100;
            EntriesCount = marketplaceEntries.Count();
            _currentCount = 0; // downloaded zero extensions so far
            await DownloadExtensionAsync(marketplaceEntries, tempDir, dte);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            dte.StatusBar.Text =
                "Extensions downloaded. Starting VSIX Installer...";
            InvokeVsixInstaller(tempDir, rootSuffix, installSystemWide);
        }

        private static void InvokeVsixInstaller(string tempDir,
            string rootSuffix, bool installSystemWide)
        {
            var vsixInstallerPath = Get.VSIXInstallerPath(
                Process.GetCurrentProcess()
            );
            if (string.IsNullOrWhiteSpace(vsixInstallerPath)) return;

            var configuration = new SetupConfiguration() as ISetupConfiguration;
            var adminSwitch = installSystemWide ? "/admin" : string.Empty;
            var instance = configuration.GetInstanceForCurrentProcess();
            var vsixFiles = Directory.EnumerateFiles(tempDir, "*.vsix")
                                     .Select(Path.GetFileName);

            var start = new ProcessStartInfo {
                FileName = vsixInstallerPath,
                Arguments =
                    $"{string.Join(" ", vsixFiles)} /instanceIds:{instance.GetInstanceId()} {adminSwitch}",
                WorkingDirectory = tempDir,
                UseShellExecute = false
            };

            /*
             * Are we being asked to install this into the Visual Studio
             * Experimental Instance?  If so, then ensure that this occurs by
             * using the /rootSuffix switch of the VSIX installer.
             */
            if (!string.IsNullOrEmpty(rootSuffix))
                start.Arguments += $" /rootSuffix:{rootSuffix}";

            Process.Start(start);
        }

        private Task DownloadExtensionAsync(IEnumerable<IGalleryEntry> entries,
            string dir, DTE dte)
        {
            /*
             * astrohart - This method has problems.  It is not fault-tolerant.
             *
             * First off, the operation of downloading an extension is not
             * guaranteed to be error-free. An I/O error could
             * be thrown by the local OS, the remote server could hiccup, or
             * any number of other things.
             *
             * I know that WhenAll starts all the tasks in the list at the same
             * time, but I want to be 100% sure that the all the rests of the tasks
             * execute even if one or more of them error out; we want to just
             * skip that one, and then continue with the others.
             *
             * Best to create a new ExtensionDownloadService or something similar,
             * have it await each individual downloading task, and then catch exceptions
             * that are thrown by that operation, so then we can skip extensions
             * that error out during their download, and still get the rest.
             *
             * This is an action item.  So that I do not break any existing
             * functionality, I am leaving it for now.
             */

            return Task.WhenAll(
                entries.Select(
                           entry => Task.Run(
                               () => DownloadExtensionFileAsync(entry, dir, dte)
                           )
                       )
                       .ToArray()
            );
        }

        private async Task DownloadExtensionFileAsync(IGalleryEntry entry,
            string dir, DTE dte)
        {
            if (entry == null) return;
            if (string.IsNullOrWhiteSpace(dir)) return;

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var localPath = Path.Combine(
                dir, GetMD5HashOf(entry.DownloadUrl) + ".vsix"
            );

            try
            {
                using (var client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(
                        entry.DownloadUrl, localPath
                    );
                }
            }
            catch
            {
                return;
            }

            await UpdateProgressAsync(dte);
        }

        private async Task UpdateProgressAsync(DTE dte)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            dte.StatusBar.Text =
                $"Downloaded {Interlocked.Increment(ref _currentCount)} of {EntriesCount} extensions...";
            await TaskScheduler.Default;
        }

        private static string GetMD5HashOf(string input)
        {
            var result = Guid.NewGuid()
                             .ToString("N");

            /*
             * MD5.Create can potentially throw a
             * TargetInvocationException.
             *
             * Therefore, it behooves us to wrap this code in a
             * try/catch block.  Since, at the end of the day, the
             * MD5 hash of a given string is unique, we substitute
             * a GUID without braces or hyphens as the output of this
             * method in the event that MD5.Create throws an exception.
             *
             * This way, callers of this method are guaranteed a unique
             * name for the file download.
             */

            try
            {
                using (var md5 = MD5.Create())
                {
                    result = BitConverter.ToString(
                                             md5.ComputeHash(
                                                 Encoding.ASCII.GetBytes(input)
                                             )
                                         )
                                         .Replace("-", string.Empty)
                                         .ToLower();
                }
            }
            catch (Exception)
            {
                // ignored

                result = Guid.NewGuid()
                             .ToString("N");
            }

            return result;
        }

        public static bool HasRootSuffix(out string rootSuffix)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var result = false;

            rootSuffix = string.Empty;

            /*
             * We wrap the code below in an exception handling block
             * because we are calling upon external code, that is not
             * guaranteed to never throw exceptions.
             */

            try
            {
                if (!(Microsoft.VisualStudio.Shell.ServiceProvider
                               .GlobalProvider
                               .GetService(typeof(SVsAppCommandLine)) is
                        IVsAppCommandLine appCommandLine)) return false;
                if (ErrorHandler.Succeeded(
                        appCommandLine.GetOption(
                            "rootsuffix", out var hasRootSuffix, out rootSuffix
                        )
                    ))
                    result = hasRootSuffix != 0;
            }
            catch
            {
                // If we are here, then something went wrong with the
                // interop connection.  Since we aren't able to gather
                // any information that is of use, return a default
                // result of false, and a empty string for the
                // rootSuffix reference.
                result = false;

                rootSuffix = string.Empty;
            }

            return result;
        }
    }
}