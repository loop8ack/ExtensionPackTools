using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Types;

internal sealed class AdapterServicesFactoryGenerator : ITypeGenerator
{
    private readonly Type _genericManagerAdapterType;
    private readonly Type _genericRepositoryAdapterType;

    public AdapterServicesFactoryGenerator(Type genericManagerAdapterType, Type genericRepositoryAdapterType)
    {
        _genericManagerAdapterType = genericManagerAdapterType;
        _genericRepositoryAdapterType = genericRepositoryAdapterType;
    }

    public Type Emit(GeneratorContext context)
    {
        return context.Emit.Class($"{context.RootNamespace}.<>VSAdapterServicesFactory",
            emit =>
            {
                emit.Implement<IVSAdapterServicesFactory>(
                    iEmit =>
                    {
                        iEmit.Method(x => x.CreateExtensionManagerAdapter())
                            .EmitIL(il =>
                            {
                                il.Emit(OpCodes.Newobj, _genericManagerAdapterType.GetConstructor([]));
                                il.Emit(OpCodes.Newobj, context.Reflect.ManagerAdapterType().GetConstructor([context.Reflect.ManagerAdapterInterfaceType()]));
                                il.Emit(OpCodes.Ret);
                            });

                        iEmit.Method(x => x.CreateExtensionRepositoryAdapter())
                            .EmitIL(il =>
                            {
                                il.Emit(OpCodes.Newobj, _genericRepositoryAdapterType.GetConstructor([]));
                                il.Emit(OpCodes.Newobj, context.Reflect.RepositoryAdapterType().GetConstructor([context.Reflect.RepositoryAdapterInterfaceType()]));
                                il.Emit(OpCodes.Ret);
                            });
                    });

                emit.DefaultCtor();
            });
    }
}

file static class Extensions
{
    public static Type ManagerAdapterInterfaceType(this GeneratorReflector reflect) => typeof(IVSExtensionManagerAdapter<,>).MakeGenericType(reflect.IVsExtensionManager(), reflect.IInstalledExtension());
    public static Type RepositoryAdapterInterfaceType(this GeneratorReflector reflect) => typeof(IVSExtensionRepositoryAdapter<>).MakeGenericType(reflect.IVsExtensionRepository());

    public static Type ManagerAdapterType(this GeneratorReflector reflect) => typeof(VSExtensionManagerAdapter<,>).MakeGenericType(reflect.IVsExtensionManager(), reflect.IInstalledExtension());
    public static Type RepositoryAdapterType(this GeneratorReflector reflect) => typeof(VSExtensionRepositoryAdapter<>).MakeGenericType(reflect.IVsExtensionRepository());
}
