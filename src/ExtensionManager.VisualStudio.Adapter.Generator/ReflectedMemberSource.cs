using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExtensionManager.VisualStudio.Adapter.Generator;

internal sealed class ReflectedMemberSource
{
    private const BindingFlags SearchBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    private readonly Dictionary<(Type type, MemberTypes memberType, string name), MemberInfo?> _members = new();
    private readonly Dictionary<string, Type?> _types = new();
    private readonly IReadOnlyList<Assembly> _assemblies;

    public ReflectedMemberSource(IReadOnlyList<Assembly> assemblies)
    {
        _assemblies = assemblies;
    }

    public Type GetType(string fullName)
    {
        if (!_types.TryGetValue(fullName, out var type))
        {
            type = GetTypeWithoutCache(fullName);
            _types.Add(fullName, type);
        }

        return type
            ?? throw new TypeLoadException($"Type {fullName} not found");
    }

    private Type? GetTypeWithoutCache(string fullName)
    {
        foreach (var assembly in _assemblies)
        {
            var type = assembly.GetType(fullName);

            if (type is not null)
                return type;
        }

        return null;
    }

    public MethodInfo GetMethod(Type type, string name)
        => (MethodInfo)GetMember(type, MemberTypes.Method, name);

    public PropertyInfo GetProperty(Type type, string name)
        => (PropertyInfo)GetMember(type, MemberTypes.Property, name);

    private MemberInfo GetMember(Type type, MemberTypes memberType, string name)
    {
        var key = (type, MemberTypes.Method, name);

        if (!_members.TryGetValue(key, out var member))
        {
            member = type.GetMember(name, memberType, SearchBindingFlags).SingleOrDefault();
            _members.Add(key, member);
        }

        return member
            ?? throw new TypeLoadException($"{memberType} {name} in {type} not found");
    }
}
