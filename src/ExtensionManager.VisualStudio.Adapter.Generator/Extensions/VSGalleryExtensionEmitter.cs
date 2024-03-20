using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Generator.Utils;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Extensions;

internal static class VSGalleryExtensionEmitter
{
    public static TypeBuilder Emit(ReflectedMemberSource source, ModuleBuilder moduleBuilder, string @namespace, string baseTypeName)
    {
        const BindingFlags searchBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        var baseType = source.GetType($"Microsoft.VisualStudio.ExtensionManager.{baseTypeName}");

        var typeBuilder = moduleBuilder.DefineType($"{@namespace}.<>VSGalleryExtension", baseType, [typeof(IVSExtension)]);

        var properties = baseType.GetProperties(searchBindingFlags);

        foreach (var property in properties)
        {
            var isAbstract = (property.GetMethod ?? property.SetMethod)?.IsAbstract ?? false;

            if (!isAbstract)
                continue;

            if (property.GetMethod is null || property.SetMethod is null)
                typeBuilder.EmitUnimplementedProperty(property);
            else
                typeBuilder.EmitAutoProperty(property);
        }

        foreach (var method in baseType.GetMethods(searchBindingFlags))
        {
            if (properties.Any(x => x.GetMethod == method || x.SetMethod == method))
                continue;

            if (!method.IsAbstract)
                continue;

            typeBuilder.EmitOverrideMethod(method,
                il => il.EmitThrow<NotImplementedException>());
        }

        typeBuilder.EmitDefaultCtor();
        typeBuilder.CreateType();

        return typeBuilder;
    }
}

file static class Extensions
{
    public static void EmitAutoProperty(this TypeBuilder typeBuilder, PropertyInfo property)
    {
        var fieldBuilder = typeBuilder
            .DefineField($"<{property.Name}>k__BackingField", property.PropertyType, FieldAttributes.Private);

        typeBuilder
            .DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null)
            .EmitPropertyAccessor(typeBuilder, property.GetMethod, isGet: true,
                il =>
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldfld, fieldBuilder);
                    il.Emit(OpCodes.Ret);
                })
            .EmitPropertyAccessor(typeBuilder, property.SetMethod, isGet: false,
                il =>
                {
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.Emit(OpCodes.Stfld, fieldBuilder);
                    il.Emit(OpCodes.Ret);
                });
    }

    public static PropertyBuilder EmitUnimplementedProperty(this TypeBuilder typeBuilder, PropertyInfo property)
    {
        return typeBuilder
            .DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null)
            .EmitPropertyAccessor(typeBuilder, property.GetMethod, isGet: true,
                static il => il.EmitThrow<NotImplementedException>())
            .EmitPropertyAccessor(typeBuilder, property.SetMethod, isGet: false,
                static il => il.EmitThrow<NotImplementedException>());
    }
}
