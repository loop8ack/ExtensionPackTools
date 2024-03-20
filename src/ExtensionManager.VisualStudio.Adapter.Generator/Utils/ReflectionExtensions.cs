using System;
using System.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Utils;

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
}
