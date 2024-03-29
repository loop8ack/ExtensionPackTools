using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public static class VisualStudioServiceFactory
    {
        public static async Task<IVisualStudioService> CreateAsync(AsyncPackage package, Version vsVersion)
        {
            if (vsVersion >= new Version(17, 10))
                return await V17_Preview.VisualStudioService.CreateAsync(package);

            if (vsVersion >= new Version(17, 9))
                return await V17.VisualStudioService.CreateAsync(package);

            throw new InvalidOperationException("Not supported visual studio version: " + vsVersion);
        }
    }
}
