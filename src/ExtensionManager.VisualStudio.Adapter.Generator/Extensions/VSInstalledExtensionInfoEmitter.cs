using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;

using ExtensionManager.VisualStudio.Adapter.Extensions;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils;
using ExtensionManager.VisualStudio.Adapter.Generator.Utils.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

internal static class VSInstalledExtensionInfoEmitter
{
    public static TypeBuilder Emit(ReflectedMemberSource source, ModuleBuilder moduleBuilder, string @namespace)
    {
        var extensionHeaderProperty = source.IInstalledExtension().GetPropertyFlatten("Header");
        var extensionIsPackComponentProperty = source.IInstalledExtension().GetPropertyFlatten("IsPackComponent");
        var extensionHeaderIdentifierProperty = extensionHeaderProperty.PropertyType.GetPropertyFlatten("Identifier");
        var extensionHeaderSystemComponentProperty = extensionHeaderProperty.PropertyType.GetPropertyFlatten("SystemComponent");

        var typeBuilder = moduleBuilder.DefineType($"{@namespace}.<>InstalledExtensionInfo", typeof(object), [typeof(IVSInstalledExtensionInfo)]);

        var fieldBuilder = typeBuilder.DefineField($"<>_extension", source.IInstalledExtension(), FieldAttributes.Private);

        typeBuilder.EmitGetOnlyProperty(Reflect<IVSInstalledExtensionInfo>.Property(x => x.Identifier),
            il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Callvirt, extensionHeaderProperty.GetMethod);
                il.Emit(OpCodes.Callvirt, extensionHeaderIdentifierProperty.GetMethod);
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitGetOnlyProperty(Reflect<IVSInstalledExtensionInfo>.Property(x => x.IsSystemComponent),
            il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Callvirt, extensionHeaderProperty.GetMethod);
                il.Emit(OpCodes.Callvirt, extensionHeaderSystemComponentProperty.GetMethod);
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitGetOnlyProperty(Reflect<IVSInstalledExtensionInfo>.Property(x => x.IsPackComponent),
            il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Callvirt, extensionIsPackComponentProperty.GetMethod);
                il.Emit(OpCodes.Ret);
            });

        typeBuilder.EmitCtorAndSetField(fieldBuilder);
        typeBuilder.CreateType();

        return typeBuilder;
    }
}

file static class Extensions
{
    public static void EmitCtorAndSetField(this TypeBuilder typeBuilder, FieldBuilder field)
    {
        const MethodAttributes ctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

        var ctorIL = typeBuilder
            .DefineConstructor(ctorAttributes, CallingConventions.Standard, [field.FieldType])
            .GetILGenerator();

        ctorIL.Emit(OpCodes.Ldarg_0);
        ctorIL.Emit(OpCodes.Call, typeof(object).GetConstructor([]));
        ctorIL.Emit(OpCodes.Ldarg_0);
        ctorIL.Emit(OpCodes.Ldarg_1);
        ctorIL.Emit(OpCodes.Stfld, field);
        ctorIL.Emit(OpCodes.Ret);

        typeBuilder.CreateType();
    }

    public static PropertyInfo GetPropertyFlatten(this Type type, string name)
    {
        return GetPropertyFlattenOrNull(type, name)
            ?? throw new ArgumentException($"Property {name} of type {type} not found");
    }
    
    private static PropertyInfo? GetPropertyFlattenOrNull(Type? type, string name)
    {
        if (type is null)
            return null;

        var property = type.GetProperty(name);

        if (property is not null)
            return property;

        foreach (var baseTypes in type.GetInterfaces())
        {
            property = GetPropertyFlattenOrNull(baseTypes, name);

            if (property is not null)
                return property;
        }

        return null;
    }
}
