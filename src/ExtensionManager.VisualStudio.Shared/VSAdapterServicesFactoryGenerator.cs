using System;
using System.Collections.Generic;
using System.Reflection;

using ExtensionManager.VisualStudio.Adapter.Generator;

#nullable enable

namespace ExtensionManager.VisualStudio;

internal sealed class VSAdapterServicesFactoryGenerator : VSAdapterServicesFactoryGeneratorBase
{
    public VSAdapterServicesFactoryGenerator(Version visualStudioVersion)
        : base(visualStudioVersion)
    {
    }

    protected override string GetGalleryExtensionBaseTypeName()
    {
#if VS2017 || VS2019
        return "GalleryOnlineExtension";
#elif VS2022
        return "OnlineExtensionBase";
#else
#error Not implemented
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
#if VS2017 || VS2019
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngine.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionManager.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionsExplorer.dll";
#elif VS2022
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngine.dll";
        yield return @$"{devenvDirectory}Microsoft.VisualStudio.ExtensionEngineContract.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionManager.dll";
        yield return @$"{devenvDirectory}PrivateAssemblies\Microsoft.VisualStudio.ExtensionsExplorer.dll";
#else
#error Not implemented
#endif
    }
}
