using System;
using System.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

internal static class ReflectionExtensions
{
    public static MethodInfo GetMethodOrThrow(this Type type, string name)
    {
        return type.GetMethod(name)
            ?? throw new InvalidOperationException($"Method not found: {type.FullName}.{name}");
    }

    public static MethodAttributes GetMethodAttributes(this MethodBase method)
    {
        var result = method switch
        {
            { IsPublic: true } => MethodAttributes.Public,
            { IsFamily: true } => MethodAttributes.Family,
            { IsAssembly: true } => MethodAttributes.Assembly,
            { IsFamilyAndAssembly: true } => MethodAttributes.FamANDAssem,
            { IsFamilyOrAssembly: true } => MethodAttributes.FamORAssem,
            { IsPrivate: true } => MethodAttributes.Private,
            _ => default
        };

        if (method.DeclaringType.IsInterface)
            result |= MethodAttributes.NewSlot;

        return result
            | MethodAttributes.SpecialName
            | MethodAttributes.HideBySig
            | MethodAttributes.Virtual
            | MethodAttributes.Final;
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
