using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Types;
using ExtensionManager.VisualStudio.Adapter.Generator.Types.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Generator;

public abstract class VSAdapterServicesFactoryGeneratorBase
{
    private readonly Version _visualStudioVersion;

    public VSAdapterServicesFactoryGeneratorBase(Version visualStudioVersion)
        => _visualStudioVersion = visualStudioVersion;

    public IVSAdapterServicesFactory Generate()
        => (IVSAdapterServicesFactory)Activator.CreateInstance(GenerateAdapterServicesFactoryType());

    private Type GenerateAdapterServicesFactoryType()
    {
        var rootNamespace = typeof(IVSAdapterServicesFactory).Namespace;
        var assemblyName = $"{rootNamespace}.{_visualStudioVersion}";

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new(assemblyName), AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);

        var assemblies = LoadVisualStudioAssemblies()
            .Concat(AppDomain.CurrentDomain.GetAssemblies())
            .ToList();

        var context = new GeneratorContext(assemblies, moduleBuilder, rootNamespace);

        var galleryExtensionType = context.EmitType(new GalleryExtensionGenerator(GetGalleryExtensionBaseTypeName()));
        var installedExtensionInfoType = context.EmitType(new InstalledExtensionInfoGenerator());
        var managerAdapterType = context.EmitType(new ExtensionManagerAdapterGenerator(installedExtensionInfoType));
        var repositoryAdapterType = context.EmitType(new ExtensionRepositoryAdapterGenerator(galleryExtensionType));

        return context.EmitType(new AdapterServicesFactoryGenerator(managerAdapterType, repositoryAdapterType));
    }

    protected abstract string GetGalleryExtensionBaseTypeName();
    protected abstract IEnumerable<Assembly> LoadVisualStudioAssemblies();
}
