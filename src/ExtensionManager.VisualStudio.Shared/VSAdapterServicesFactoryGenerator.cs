using System;
using System.Collections.Generic;
using System.Reflection;

using ExtensionManager.VisualStudio.Adapter.Generator;

#if V15
namespace ExtensionManager.VisualStudio.V15;
#elif V16
namespace ExtensionManager.VisualStudio.V16;
#elif V17
namespace ExtensionManager.VisualStudio.V17;
#endif

internal sealed class VSAdapterServicesFactoryGenerator : VSAdapterServicesFactoryGeneratorBase
{
    public VSAdapterServicesFactoryGenerator(Version visualStudioVersion)
        : base(visualStudioVersion)
    {
    }

    protected override string GetGalleryExtensionBaseTypeName()
    {
#if V15 || V16
        return "GalleryOnlineExtension";
#elif V17
        return "OnlineExtensionBase";
#else
        throw new NotImplementedException();
#endif
    }

    protected override IEnumerable<Assembly> LoadVisualStudioAssemblies()
    {
        var devenvDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR");

        foreach (var filePath in GetVisualStudioAssemblyFilePaths(devenvDirectory))
            yield return Assembly.LoadFile(filePath);
    }
    
    private IEnumerable<string> GetVisualStudioAssemblyFilePaths(string devenvDirectory)
    {
#if V15 || V16
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngine.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionManager.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionsExplorer.dll";
#elif V17
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngine.dll";
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngineContract.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionManager.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionsExplorer.dll";
#else
        throw new NotImplementedException();
#endif
    }
}
