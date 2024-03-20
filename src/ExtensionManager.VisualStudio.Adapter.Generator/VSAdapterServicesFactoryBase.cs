using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Generator;

public abstract class VSAdapterServicesFactoryBase : IVSAdapterServicesFactory
{
    private static string RootNamespace => typeof(IVSAdapterServicesFactory).Namespace;

    private readonly Lazy<(Assembly assembly, IVSAdapterServicesFactory generatedFactory)> _lazy;
    private readonly Version _visualStudioVersion;

    public VSAdapterServicesFactoryBase(Version visualStudioVersion)
    {
        _lazy = new Lazy<(Assembly assembly, IVSAdapterServicesFactory generatedFactory)>(Generate);
        _visualStudioVersion = visualStudioVersion;
    }

    public IVSExtensionManagerAdapter CreateExtensionManagerAdapter()
        => _lazy.Value.generatedFactory.CreateExtensionManagerAdapter();

    public IVSExtensionRepositoryAdapter CreateExtensionRepositoryAdapter()
        => _lazy.Value.generatedFactory.CreateExtensionRepositoryAdapter();

    private (Assembly assembly, IVSAdapterServicesFactory generatedFactory) Generate()
    {
        var assembly = GenerateAssembly($"{RootNamespace}.{_visualStudioVersion}");

        var servicesFactoryType = assembly.GetTypes()
            .Where(typeof(IVSAdapterServicesFactory).IsAssignableFrom)
            .Single();

        var generatedFactory = (IVSAdapterServicesFactory)Activator.CreateInstance(servicesFactoryType);

        return (assembly, generatedFactory);
    }

    private Assembly GenerateAssembly(string assemblyName)
    {
        var extensionsRootNamespace = $"{RootNamespace}.Extensions";

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new(assemblyName), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

        var assemblies = LoadVisualStudioAssemblies().ToList();
        assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        var memberSource = new ReflectedMemberSource(assemblies);

        var galleryExtensionBaseTypeName = GetGalleryExtensionBaseTypeName();

        var galleryExtensionTypeBuilder = VSGalleryExtensionEmitter.Emit(memberSource, moduleBuilder, extensionsRootNamespace, galleryExtensionBaseTypeName);
        var installedExtensionInfoTypeBuilder = VSInstalledExtensionInfoEmitter.Emit(memberSource, moduleBuilder, extensionsRootNamespace);
        var managerAdapterTypeBuilder = VSExtensionManagerAdapterEmitter.Emit(memberSource, moduleBuilder, extensionsRootNamespace, installedExtensionInfoTypeBuilder);
        var repositoryAdapterTypeBuilder = VSExtensionRepositoryAdapterEmitter.Emit(memberSource, moduleBuilder, extensionsRootNamespace, galleryExtensionTypeBuilder);
        VSAdapterServicesFactoryEmitter.Emit(memberSource, moduleBuilder, RootNamespace, managerAdapterTypeBuilder, repositoryAdapterTypeBuilder);

        return assemblyBuilder;
    }

    protected abstract string GetGalleryExtensionBaseTypeName();
    protected abstract IEnumerable<Assembly> LoadVisualStudioAssemblies();
}
