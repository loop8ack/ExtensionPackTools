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
    /// <summary>
    /// Exposes methods to execute a command that allows the user to export the list of
    /// extension(s) that are required to be installed in Visual Studio in order to
    /// successfully build a given solution.
    /// </summary>
    internal sealed class ExportSolutionCommand : CommandBase
    {
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
                PackageIds.ExportSolutionCmd,
                /* supported = */ false
            );
        }

        public static ExportSolutionCommand Instance { get; private set; }

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
            IMenuCommandService commandService,
            IExtensionService extensionService)
        {
            Instance = new ExportSolutionCommand(
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

            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            if (dte == null)
                ShowMessageBox(
                    "We could not connect to Visual Studio.\n\nSave your work, and then try the operation again.\n\nIf this error recurs, please seek support."
                );

            if (string.IsNullOrEmpty(dte.Solution?.FileName))
            {
                ShowMessageBox(
                    "We can't export the list of extensions that this solution requires.\n\nIf you've created a new solution, choose the File Save All menu item, and then try this operation again."
                );
                return;
            }

            var fileName = Path.ChangeExtension(
                dte.Solution?.FileName, ".vsext"
            );

            try
            {
                /*
                 * OKAY, so before we show the user a prompt to select which extensions
                 * they want to export, we first need to determine the value set of the
                 * extensions to choose from.
                 *
                 * This will be the union set of the extensions that are currently
                 * installed in this running instance of Visual Studio, taken together
                 * with whatever extensions may already be listed in a file, e.g., already
                 * sitting in the Solution Items that may have previously been exported.
                 */

                var extensions = _extensionService.GetInstalledExtensions()
                                                  .ToList();

                Manifest manifest;

                if (File.Exists(fileName))
                {
                    manifest = Manifest.FromFile(fileName);
                    extensions = extensions.Union(manifest.Extensions)
                                           .ToList();

                    foreach (var ext in extensions)
                        ext.Selected = manifest.Extensions.Contains(ext);
                }
                else
                {
                    foreach (var ext in extensions) ext.Selected = false;
                }

                var dialog = ImportWindow.Open(extensions, Purpose.Export);

                if (dialog.DialogResult != true)
                    return;

                manifest = new Manifest(dialog.SelectedExtensions);
                var json = JsonConvert.SerializeObject(
                    manifest, Formatting.Indented
                );

                File.WriteAllText(fileName, json);

                // Add the file to the solution items folder if it's new or if it's not there already.
                var solutionItemsFolder = GetOrCreateSolutionItems((DTE2)dte);
                solutionItemsFolder.ProjectItems.AddFromFile(fileName);

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

            var solutionItemsFolder = dte.Solution.Projects.Cast<Project>()
                                         .FirstOrDefault(
                                             p =>
                                             {
                                                 ThreadHelper
                                                     .ThrowIfNotOnUIThread();
                                                 return p.Name ==
                                                     "Solution Items" ||
                                                     p.Kind == Constants
                                                         .vsProjectItemKindSolutionItems;
                                             }
                                         );

            if (solutionItemsFolder != null)
                return solutionItemsFolder;

            var solution2 = (Solution2)dte.Solution;
            solutionItemsFolder = solution2.AddSolutionFolder("Solution Items");
            dte.StatusBar.Text =
                $"Created Solution Items project for solution {dte.Solution.FullName}";

            return solutionItemsFolder;
        }
    }
}