using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public class SolutionPrompter
    {
        /// <summary>
        /// Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </summary>
        private readonly IExtensionService _extensionService;

        /// <summary>
        /// Constructs a new instance of <see cref="T:ExtensionManager.SolutionPrompter" />
        /// and returns a reference to it.
        /// </summary>
        /// <param name="extensionService">
        /// (Required.) Reference to an instance of an object that implements the
        /// <see cref="T:ExtensionManager.IExtensionService" /> interface.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Thrown if the required
        /// parameter, <paramref name="extensionService" />, is passed a
        /// <see langword="null" /> value.
        /// </exception>
        public SolutionPrompter(IExtensionService extensionService)
        {
            _extensionService = extensionService ??
                                throw new ArgumentNullException(
                                    nameof(extensionService)
                                );
        }

        public void Check(string fileName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!File.Exists(fileName)) return;

            var manifest = Manifest.FromFile(fileName);

            var installedExtensions =
                _extensionService.GetInstalledExtensions();
            manifest.MarkSelected(installedExtensions);

            if (!manifest.Extensions.Any(e => e.Selected))
                return;

            ImportWindow.Open(
                manifest.Extensions, Purpose.InstallForSolution,
                "This solution asks that you install the following extensions."
            );
        }
    }
}