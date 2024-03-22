using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Types.Extensions;

internal sealed class ExtensionRepositoryAdapterGenerator : ITypeGenerator
{
    private readonly Type _galleryExtensionType;

    public ExtensionRepositoryAdapterGenerator(Type galleryExtensionType)
        => _galleryExtensionType = galleryExtensionType;

    public Type Emit(GeneratorContext context)
    {
        return context.Emit.Class($"{context.RootNamespace}.Extensions.<>VSExtensionRepositoryAdapter",
            emit =>
            {
                emit.Implement(context.Reflect.AdapterInterface());

                emit.ImplementMethod(context.Reflect.Adapter_GetRepositoryAsync())
                    .EmitIL(il =>
                    {
                        il.Emit(OpCodes.Call, context.Reflect.VS_GetRepositoryServiceAsync());
                        il.Emit(OpCodes.Ret);
                    });

                emit.ImplementMethod(context.Reflect.Adapter_GetVSGalleryExtensions())
                    .EmitIL(il =>
                    {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Ldarg_3);
                        il.Emit(OpCodes.Ldarg, 4);
                        il.Emit(OpCodes.Callvirt, context.Reflect.Repository_GetVSGalleryExtensions(_galleryExtensionType));
                        il.Emit(OpCodes.Ret);
                    });

                emit.DefaultCtor();
            });
    }
}

file static class Extensions
{
    public static Type AdapterInterface(this GeneratorReflector reflect) => typeof(IVSExtensionRepositoryAdapter<>).MakeGenericType(reflect.IVsExtensionRepository());

    public static MethodInfo Adapter_GetRepositoryAsync(this GeneratorReflector reflect) => reflect.AdapterInterface().GetMethod(nameof(IVSExtensionRepositoryAdapter<int>.GetRepositoryAsync));
    public static MethodInfo Adapter_GetVSGalleryExtensions(this GeneratorReflector reflect) => reflect.AdapterInterface().GetMethod(nameof(IVSExtensionRepositoryAdapter<int>.GetVSGalleryExtensions));

    public static MethodInfo VS_GetRepositoryServiceAsync(this GeneratorReflector reflect) => reflect.VS_GetRequiredServiceAsync(reflect.SVsExtensionRepository(), reflect.IVsExtensionRepository());

    public static MethodInfo Repository_GetVSGalleryExtensions(this GeneratorReflector reflect, Type extensionType) => reflect.IVsExtensionRepository().GetMethodOrThrow("GetVSGalleryExtensions").MakeGenericMethod(extensionType);
}
