using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using ExtensionManager.Core.Services.Interfaces;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;
using Constants = EnvDTE.Constants;

namespace ExtensionManager
{
    internal sealed class ExportSolutionCommand
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </summary>
        private readonly IExtensionService _extensionService;

        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:Microsoft.VisualStudio.Shell.Interop.IVsPackage" /> interface.
        /// </summary>
        private readonly IVsPackage _package;

        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.ExportSolutionCommand" /> and returns a reference
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
        private ExportSolutionCommand(IVsPackage package,
            IMenuCommandService commandService,
            IExtensionService extensionService)
        {
            if (commandService == null)
                throw new ArgumentNullException(nameof(commandService));

            _package = package ??
                       throw new ArgumentNullException(nameof(package));
            _extensionService = extensionService ??
                                throw new ArgumentNullException(
                                    nameof(extensionService)
                                );

            var cmdId = new CommandID(
                PackageGuids.guidExportPackageCmdSet,
                PackageIds.ExportSolutionCmd
            );
            var cmd = new MenuCommand(Execute, cmdId) { Supported = false };

            commandService.AddCommand(cmd);
        }

        public static ExportSolutionCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
            => (IServiceProvider)_package;

        /// <summary>
        /// Creates and initializes a new instance of
        /// <see cref="T:ExtensionManager.ExportSolutionCommand" /> and sets the
        /// <see cref="P:ExtensionManager.ExportSolutionCommand.Instance" /> property to a
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
            IMenuCommandService commandService, IExtensionService extensionService)
        {
            Instance = new ExportSolutionCommand(package, commandService, extensionService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            if (string.IsNullOrEmpty(dte.Solution?.FileName))
            {
                ShowMessageBox(
                    "The solution must be saved in order to manage solution extensions."
                );
                return;
            }

            var fileName = Path.ChangeExtension(
                dte.Solution?.FileName, ".vsext"
            );

            try
            {
                var extensions = _extensionService.GetInstalledExtensions()
                                                  .ToList();

                Manifest manifest;

                if (File.Exists(fileName))
                {
                    manifest = Manifest.FromFile(fileName);
                    extensions = extensions.Union(manifest.Extensions)
                                           .ToList();

                    foreach (Extension ext in extensions)
                        ext.Selected = manifest.Extensions.Contains(ext);
                }
                else
                {
                    foreach (Extension ext in extensions) ext.Selected = false;
                }

                var dialog = ImportWindow.Open(extensions, Purpose.Export);

                if (dialog.DialogResult != true)
                    return;
                
                manifest = new Manifest(dialog.SelectedExtension);
                var json = JsonConvert.SerializeObject(
                    manifest, Formatting.Indented
                );

                File.WriteAllText(fileName, json);

                // Add the file to the solution items folder if it's new or if it's not there already.
                var solItems = GetOrCreateSolutionItems((DTE2)dte);
                solItems.ProjectItems.AddFromFile(fileName);

                VsShellUtilities.OpenDocument(ServiceProvider, fileName);
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void ShowMessageBox(string message)
        {
            VsShellUtilities.ShowMessageBox(
                ServiceProvider, message, Vsix.Name,
                OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
            );
        }

        /// <summary>
        /// Gets or creates solution items folder (project).
        /// from https://blog.agchapman.com/creating-solution-items-from-vs-extension/
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <returns>the solution items folder (project)</returns>
        private static Project GetOrCreateSolutionItems(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solItems = dte.Solution.Projects.Cast<Project>()
                              .FirstOrDefault(
                                  p =>
                                  {
                                      ThreadHelper.ThrowIfNotOnUIThread();
                                      return p.Name == "Solution Items" ||
                                             p.Kind == Constants
                                                 .vsProjectItemKindSolutionItems;
                                  }
                              );

            if (solItems == null)
            {
                var sol2 = (Solution2)dte.Solution;
                solItems = sol2.AddSolutionFolder("Solution Items");
                dte.StatusBar.Text =
                    $"Created Solution Items project for solution {dte.Solution.FullName}";
            }

            return solItems;
        }
    }
}