using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

internal static class VSExtensionRepositoryAdapterEmitter
{
    public static TypeBuilder Emit(ReflectedMemberSource source, ModuleBuilder moduleBuilder, string @namespace, TypeBuilder galleryExtensionType)
    {
        var adapterInterfaceType = typeof(IVSExtensionRepositoryAdapter<>).MakeGenericType(source.IVsExtensionRepository());

        var typeBuilder = moduleBuilder.DefineType($"{@namespace}.<>VSExtensionRepositoryAdapter", typeof(object), [adapterInterfaceType]);

        typeBuilder.EmitOverrideMethod(adapterInterfaceType.GetMethod(nameof(IVSExtensionRepositoryAdapter<int>.GetRepositoryAsync)),
            il =>
            {
                il.Emit(OpCodes.Call, source.VS_GetRequiredServiceAsync(source.SVsExtensionRepository(), source.IVsExtensionRepository()));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitOverrideMethod(adapterInterfaceType.GetMethod(nameof(IVSExtensionRepositoryAdapter<int>.GetVSGalleryExtensions)),
            il =>
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                il.Emit(OpCodes.Ldarg, 4);
                il.Emit(OpCodes.Callvirt, source.IVsExtensionRepository_GetVSGalleryExtensions(galleryExtensionType));
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitDefaultCtor();
        typeBuilder.CreateType();

        return typeBuilder;
    }
}
