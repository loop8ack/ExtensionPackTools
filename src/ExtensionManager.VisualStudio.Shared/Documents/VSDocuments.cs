using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

namespace ExtensionManager.VisualStudio.Documents;

internal sealed class VSDocuments : IVSDocuments
{
    public Task OpenAsync(string file) => VS.Documents.OpenAsync(file);
}
