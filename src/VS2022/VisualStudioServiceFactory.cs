using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace ExtensionManager
{
    public static class VisualStudioServiceFactory
    {
        public static async Task<IVisualStudioService> CreateAsync(AsyncPackage package, Version vsVersion)
        {
            if (vsVersion >= new Version(17, 7))
                return await V17_7.VisualStudioService.CreateAsync(package);

            if (vsVersion >= new Version(17, 5))
                return await V17_5.VisualStudioService.CreateAsync(package);

            throw new InvalidOperationException("Not supported visual studio version: " + vsVersion);
        }
    }
}
