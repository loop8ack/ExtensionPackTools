using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public static class VisualStudioServiceFactory
    {
        public static async Task<IVisualStudioService> CreateAsync(AsyncPackage package, Version vsVersion)
        {
            return await VisualStudioService.CreateAsync(package);
        }
    }
}
