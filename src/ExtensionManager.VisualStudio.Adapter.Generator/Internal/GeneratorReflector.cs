using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal;

internal sealed class GeneratorReflector
{
    private const BindingFlags SearchBindingFlags = 0
        | BindingFlags.Public
        | BindingFlags.NonPublic
        | BindingFlags.Instance
        | BindingFlags.Static;

    private readonly IReadOnlyList<Assembly> _assemblies;

    public GeneratorReflector(IReadOnlyList<Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    public Type GetType(string fullName)
    {
        foreach (var assembly in _assemblies)
        {
            var type = assembly.GetType(fullName);

            if (type is not null)
                return type;
        }

        throw new TypeLoadException($"Type {fullName} not found");
    }

    public MethodInfo GetMethod(Type type, string name)
        => (MethodInfo)GetMember(type, MemberTypes.Method, name);

    public PropertyInfo GetProperty(Type type, string name)
        => (PropertyInfo)GetMember(type, MemberTypes.Property, name);

    private MemberInfo GetMember(Type type, MemberTypes memberType, string name)
    {
        return type.GetMember(name, memberType, SearchBindingFlags).SingleOrDefault()
            ?? throw new TypeLoadException($"{memberType} {name} in {type} not found");
    }

    public Type IInstalledExtension() => GetType("Microsoft.VisualStudio.ExtensionManager.IInstalledExtension");
    public Type IVsExtensionManager() => GetType("Microsoft.VisualStudio.ExtensionManager.IVsExtensionManager");
    public Type SVsExtensionManager() => GetType("Microsoft.VisualStudio.ExtensionManager.SVsExtensionManager");
    public Type IVsExtensionRepository() => GetType("Microsoft.VisualStudio.ExtensionManager.IVsExtensionRepository");
    public Type SVsExtensionRepository() => GetType("Microsoft.VisualStudio.ExtensionManager.SVsExtensionRepository");

    public MethodInfo VS_GetRequiredServiceAsync(Type serviceType, Type interfaceType)
    {
        return GetType("Community.VisualStudio.Toolkit.VS")
            .GetMethodOrThrow("GetRequiredServiceAsync")
            .MakeGenericMethod(serviceType, interfaceType);
    }
}
