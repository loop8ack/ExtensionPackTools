using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using ExtensionManager.Importer;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Newtonsoft.Json;

namespace ExtensionManager
{
    internal sealed class ExportSolutionCommand
    {
        private readonly Package _package;
        private readonly ExtensionService _es;

        private ExportSolutionCommand(Package package, OleMenuCommandService commandService, ExtensionService es)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _es = es;

            var cmdId = new CommandID(PackageGuids.guidExportPackageCmdSet, PackageIds.ExportSolutionCmd);
            var cmd = new MenuCommand(Execute, cmdId)
            {
                Supported = false
            };

            commandService.AddCommand(cmd);
        }

        public static ExportSolutionCommand Instance { get; private set; }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package, OleMenuCommandService commandService, ExtensionService es)
        {
            Instance = new ExportSolutionCommand(package, commandService, es);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = ServiceProvider.GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            if (string.IsNullOrEmpty(dte.Solution?.FileName))
            {
                ShowMessageBox("The solution must be saved in order to manage solution extensions.");
                return;
            }

            string fileName = Path.ChangeExtension(dte.Solution.FileName, ".vsext");

            try
            {
                var extensions = _es.GetInstalledExtensions().ToList();

                if (File.Exists(fileName))
                {
                    var manifest = Manifest.FromFile(fileName);
                    extensions = extensions.Union(manifest.Extensions).ToList();

                    foreach (Extension ext in extensions)
                    {
                        ext.Selected = manifest.Extensions.Contains(ext);
                    }
                }
                else
                {
                    foreach (Extension ext in extensions)
                    {
                        ext.Selected = false;
                    }
                }

                var dialog = ImportWindow.Open(extensions, Purpose.List);

                if (dialog.DialogResult == true)
                {
                    var manifest = new Manifest(dialog.SelectedExtension);
                    string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

                    File.WriteAllText(fileName, json);

                    // Add the file to the solution items folder if it's new or if it's not there already.
                    var solItems = GetOrCreateSolutionItems((DTE2)dte);
                    solItems.ProjectItems.AddFromFile(fileName);

                    VsShellUtilities.OpenDocument(ServiceProvider, fileName);
                }
            }
            catch (Exception ex)
            {
                ShowMessageBox(ex.Message);
            }
        }

        private void ShowMessageBox(string message)
        {
            VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    message,
                    Vsix.Name,
                    OLEMSGICON.OLEMSGICON_WARNING,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                );
        }

        private bool TryGetFilePath(out string filePath)
        {
            filePath = null;

            using (var sfd = new SaveFileDialog())
            {
                sfd.DefaultExt = ".vsext";
                sfd.FileName = "extensions";
                sfd.Filter = "VSEXT File|*.vsext";

                DialogResult result = sfd.ShowDialog();

                if (result == DialogResult.OK)
                {
                    filePath = sfd.FileName;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets or creates solution items folder (project).
        /// from https://blog.agchapman.com/creating-solution-items-from-vs-extension/
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <returns>the solution items folder (project)</returns>
        static Project GetOrCreateSolutionItems(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solItems = dte.Solution.Projects.Cast<Project>().FirstOrDefault(p => p.Name == "Solution Items" || p.Kind == EnvDTE.Constants.vsProjectItemKindSolutionItems);
            if (solItems == null)
            {
                Solution2 sol2 = (Solution2)dte.Solution;
                solItems = sol2.AddSolutionFolder("Solution Items");
                dte.StatusBar.Text = $"Created Solution Items project for solution {dte.Solution.FullName}";
            }
            return solItems;
        }
    }
}
