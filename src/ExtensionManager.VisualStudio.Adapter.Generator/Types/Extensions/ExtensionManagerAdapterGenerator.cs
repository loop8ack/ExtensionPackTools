using System;
using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Types.Extensions;

internal sealed class ExtensionManagerAdapterGenerator : ITypeGenerator
{
    private readonly Type _installedExtensionInfoType;

    public ExtensionManagerAdapterGenerator(Type installedExtensionInfoType)
        => _installedExtensionInfoType = installedExtensionInfoType;

    public Type Emit(GeneratorContext context)
    {
        return context.Emit.Class($"{context.RootNamespace}.Extensions.<>VSExtensionManagerAdapter",
            emit =>
            {
                emit.Implement(context.Reflect.AdapterInterface());

                emit.ImplementMethod(context.Reflect.Adapter_GetManagerAsync())
                    .EmitIL(il =>
                    {
                        il.Emit(OpCodes.Call, context.Reflect.VS_GetManagerServiceAsync());
                        il.Emit(OpCodes.Ret);
                    });

                emit.ImplementMethod(context.Reflect.Adapter_GetInstalledExtensions())
                    .EmitIL(il =>
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Callvirt, context.Reflect.Manager_GetInstalledExtensions());
                        il.Emit(OpCodes.Ret);
                    });

                emit.ImplementMethod(context.Reflect.Adapter_CreateInstalledExtensionInfo())
                    .EmitIL(il =>
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Newobj, _installedExtensionInfoType.GetConstructor([context.Reflect.IInstalledExtension()]));
                        il.Emit(OpCodes.Ret);
                    });

                emit.DefaultCtor();
            });
    }
}

file static class Extensions
{
    public static Type AdapterInterface(this GeneratorReflector reflect) => typeof(IVSExtensionManagerAdapter<,>).MakeGenericType(reflect.IVsExtensionManager(), reflect.IInstalledExtension());

    public static MethodInfo Adapter_GetManagerAsync(this GeneratorReflector reflect) => reflect.AdapterInterface().GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.GetManagerAsync));
    public static MethodInfo Adapter_GetInstalledExtensions(this GeneratorReflector reflect) => reflect.AdapterInterface().GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.GetInstalledExtensions));
    public static MethodInfo Adapter_CreateInstalledExtensionInfo(this GeneratorReflector reflect) => reflect.AdapterInterface().GetMethod(nameof(IVSExtensionManagerAdapter<int, int>.CreateInstalledExtensionInfo));

    public static MethodInfo VS_GetManagerServiceAsync(this GeneratorReflector reflect) => reflect.VS_GetRequiredServiceAsync(reflect.SVsExtensionManager(), reflect.IVsExtensionManager());

    public static MethodInfo Manager_GetInstalledExtensions(this GeneratorReflector reflect) => reflect.IVsExtensionManager().GetMethodOrThrow("GetInstalledExtensions");
}
