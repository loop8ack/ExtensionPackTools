using System;
using System.ComponentModel.Design;
using System.IO;
using ExtensionManager.Core.Services.Interfaces;
using ExtensionManager.Shared.Actions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;

namespace ExtensionManager
{
    /// <summary>
    /// Command that the user invokes to export the list of their installed extensions.
    /// </summary>
    internal sealed class ExportCommand : CommandBase
    {
        /// <summary>
        /// Constructs a new instance of
        /// <see cref="T:ExtensionManager.ExportCommand" /> and returns a reference
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
        private ExportCommand(IVsPackage package,
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
                PackageIds.ExportCmd
            );
        }

        public static ExportCommand Instance { get; private set; }

        /// <summary>
        /// Creates and initializes a new instance of
        /// <see cref="T:ExtensionManager.ExportCommand" /> and sets the
        /// <see cref="P:ExtensionManager.ExportCommand.Instance" /> property to a
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
            Instance = new ExportCommand(
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

            try
            {
                var dialog = ImportWindow.Open(
                    InstalledExtensions, Purpose.Export
                );

                if (dialog.DialogResult != true)
                    return;

                var exportFilePath = Get.ExportFilePath();
                if (string.IsNullOrWhiteSpace(exportFilePath))
                    return;

                File.WriteAllText(
                    exportFilePath,
                    JsonConvert.SerializeObject(
                        new Manifest(dialog.SelectedExtensions),
                        Formatting.Indented
                    )
                );

                VsShellUtilities.OpenDocument(ServiceProvider, exportFilePath);
            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    $"{ex.Message}\n\nTry running the Export Extensions operation again.\n\nBe sure you have write privileges in the folder to which you are saving the export.", Vsix.Name,
                    OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
            }
        }
    }
}