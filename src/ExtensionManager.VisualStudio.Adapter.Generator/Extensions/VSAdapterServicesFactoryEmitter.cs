using System;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

internal static class VSAdapterServicesFactoryEmitter
{
    public static Type Emit(ReflectedMemberSource source, ModuleBuilder moduleBuilder, string @namespace, TypeBuilder genericManagerAdapterType, TypeBuilder genericRepositoryAdapterType)
    {
        var managerAdapterInterfaceType = typeof(IVSExtensionManagerAdapter<,>).MakeGenericType(source.IVsExtensionManager(), source.IInstalledExtension());
        var repositoryAdapterInterfaceType = typeof(IVSExtensionRepositoryAdapter<>).MakeGenericType(source.IVsExtensionRepository());

        var managerAdapterType = typeof(VSExtensionManagerAdapter<,>).MakeGenericType(source.IVsExtensionManager(), source.IInstalledExtension());
        var repositoryAdapterType = typeof(VSExtensionRepositoryAdapter<>).MakeGenericType(source.IVsExtensionRepository());

        var typeBuilder = moduleBuilder.DefineType($"{@namespace}.<>VSAdapterServicesFactory", typeof(object), [typeof(IVSAdapterServicesFactory)]);

        typeBuilder.EmitOverrideMethod(Reflect<IVSAdapterServicesFactory>.Method(x => x.CreateExtensionManagerAdapter()),
            il =>
            {
                il.Emit(OpCodes.Newobj, genericManagerAdapterType.GetConstructor([]));
                il.Emit(OpCodes.Newobj, managerAdapterType.GetConstructor([managerAdapterInterfaceType]));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitOverrideMethod(Reflect<IVSAdapterServicesFactory>.Method(x => x.CreateExtensionRepositoryAdapter()),
            il =>
            {
                il.Emit(OpCodes.Newobj, genericRepositoryAdapterType.GetConstructor([]));
                il.Emit(OpCodes.Newobj, repositoryAdapterType.GetConstructor([repositoryAdapterInterfaceType]));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitDefaultCtor();
        typeBuilder.CreateType();

        return typeBuilder;
    }
}
