using System;
using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Types.Extensions;

internal sealed class InstalledExtensionInfoGenerator : ITypeGenerator
{
    public Type Emit(GeneratorContext context)
    {
        return context.Emit.Class($"{context.RootNamespace}.Extensions.<>InstalledExtensionInfo",
            emit =>
            {
                var fieldBuilder = emit.Field($"<>_extension", context.Reflect.IInstalledExtension(), FieldAttributes.Private | FieldAttributes.InitOnly);

                emit.Implement<IVSInstalledExtensionInfo>(
                    iEmit =>
                    {
                        iEmit.Property(x => x.Identifier,
                            (pEmit, p) => pEmit.Get(p.GetMethod.GetMethodAttributes())
                                .EmitIL(il =>
                                {
                                    il.Emit(OpCodes.Ldarg_0);
                                    il.Emit(OpCodes.Ldfld, fieldBuilder);
                                    il.Emit(OpCodes.Callvirt, context.Reflect.InstalledExtension_Header().GetMethod);
                                    il.Emit(OpCodes.Callvirt, context.Reflect.InstalledExtension_Header_Identifier().GetMethod);
                                    il.Emit(OpCodes.Ret);
                                }));

                        iEmit.Property(x => x.IsSystemComponent,
                            (pEmit, p) => pEmit.Get(p.GetMethod.GetMethodAttributes())
                                .EmitIL(il =>
                                {
                                    il.Emit(OpCodes.Ldarg_0);
                                    il.Emit(OpCodes.Ldfld, fieldBuilder);
                                    il.Emit(OpCodes.Callvirt, context.Reflect.InstalledExtension_Header().GetMethod);
                                    il.Emit(OpCodes.Callvirt, context.Reflect.InstalledExtension_Header_SystemComponent().GetMethod);
                                    il.Emit(OpCodes.Ret);
                                }));

                        iEmit.Property(x => x.IsPackComponent,
                            (pEmit, p) => pEmit.Get(p.GetMethod.GetMethodAttributes())
                                .EmitIL(il =>
                                {
                                    il.Emit(OpCodes.Ldarg_0);
                                    il.Emit(OpCodes.Ldfld, fieldBuilder);
                                    il.Emit(OpCodes.Callvirt, context.Reflect.InstalledExtension_IsPackComponent().GetMethod);
                                    il.Emit(OpCodes.Ret);
                                }));
                    });

                emit.AutoCtor(fieldBuilder);
            });
    }
}

file static class Extensions
{
    public static PropertyInfo InstalledExtension_Header(this GeneratorReflector reflect) => reflect.IInstalledExtension().GetPropertyFlatten("Header");
    public static PropertyInfo InstalledExtension_Header_Identifier(this GeneratorReflector reflect) => reflect.InstalledExtension_Header().PropertyType.GetPropertyFlatten("Identifier");
    public static PropertyInfo InstalledExtension_Header_SystemComponent(this GeneratorReflector reflect) => reflect.InstalledExtension_Header().PropertyType.GetPropertyFlatten("SystemComponent");
    public static PropertyInfo InstalledExtension_IsPackComponent(this GeneratorReflector reflect) => reflect.IInstalledExtension().GetPropertyFlatten("IsPackComponent");
}
