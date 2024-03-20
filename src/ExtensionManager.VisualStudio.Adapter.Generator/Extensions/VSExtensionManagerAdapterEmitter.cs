using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

internal static class VSExtensionManagerAdapterEmitter
{
    public static TypeBuilder Emit(ReflectedMemberSource source, ModuleBuilder moduleBuilder, string @namespace, TypeBuilder installedExtensionInfoType)
    {
        var adapterInterfaceType = typeof(IVSExtensionManagerAdapter<,>).MakeGenericType(source.IVsExtensionManager(), source.IInstalledExtension());

        var typeBuilder = moduleBuilder.DefineType($"{@namespace}.<>VSExtensionManagerAdapter", typeof(object), [adapterInterfaceType]);

        typeBuilder.EmitMethod(adapterInterfaceType.GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.GetManagerAsync)),
            il =>
            {
                il.Emit(OpCodes.Call, source.VS_GetRequiredServiceAsync(source.SVsExtensionManager(), source.IVsExtensionManager()));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitMethod(adapterInterfaceType.GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.GetInstalledExtensions)),
            il =>
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Callvirt, source.IVsExtensionManager_GetInstalledExtensions());
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitMethod(adapterInterfaceType.GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.CreateInstalledExtensionInfo)),
            il =>
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Newobj, installedExtensionInfoType.GetConstructor([source.IInstalledExtension()]));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitDefaultCtor();
        typeBuilder.CreateType();

        return typeBuilder;
    }
}
