using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Utils;

internal static class ReflectionEmitExtensions
{
    private const TypeAttributes ClassTypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class | TypeAttributes.BeforeFieldInit;

    public static void EmitDefaultCtor(this TypeBuilder typeBuilder)
    {
        const BindingFlags searchCtorAttributes = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        const MethodAttributes ctorAttributes = MethodAttributes.Public
            | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName
            | MethodAttributes.RTSpecialName;

        var il = typeBuilder
            .DefineConstructor(ctorAttributes, CallingConventions.Standard, [])
            .GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, typeBuilder.BaseType.GetConstructor(searchCtorAttributes, null, [], []));
        il.Emit(OpCodes.Ret);
    }

    public static TypeBuilder DefineType(this ModuleBuilder moduleBuilder, string name, Type parent, params Type[] interfaces)
    {
        return moduleBuilder.DefineType(name, ClassTypeAttributes, parent, interfaces);
    }

    public static MethodBuilder EmitMethod(this TypeBuilder typeBuilder, MethodInfo baseMethod, Action<ILGenerator> generateIL)
    {
        var methodBuilder = typeBuilder.DefineMethod(baseMethod.Name,
            attributes: baseMethod.GetMethodAttributes(),
            returnType: baseMethod.ReturnType == typeof(void) ? null : baseMethod.ReturnType,
            parameterTypes: baseMethod.GetParameters().SelectArray(x => x.ParameterType));

        generateIL(methodBuilder.GetILGenerator());

        return methodBuilder;
    }

    public static MethodBuilder EmitOverrideMethod(this TypeBuilder typeBuilder, MethodInfo baseMethod, Action<ILGenerator> generateIL)
    {
        var methodBuilder = EmitMethod(typeBuilder, baseMethod, generateIL);

        typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);

        return methodBuilder;
    }

    public static PropertyBuilder EmitGetOnlyProperty(this TypeBuilder typeBuilder, PropertyInfo property, Action<ILGenerator> generateIL)
    {
        return typeBuilder
            .DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null)
            .EmitPropertyAccessor(typeBuilder, property.GetMethod, isGet: true, generateIL);
    }

    public static PropertyBuilder EmitPropertyAccessor(this PropertyBuilder propertyBuilder, TypeBuilder typeBuilder, MethodInfo? accessorMethod, bool isGet, Action<ILGenerator> generateIL)
    {
        if (accessorMethod is null)
            return propertyBuilder;

        var methodBuilder = typeBuilder.EmitMethod(accessorMethod, generateIL);

        if (isGet)
            propertyBuilder.SetGetMethod(methodBuilder);
        else
            propertyBuilder.SetSetMethod(methodBuilder);

        return propertyBuilder;
    }

    public static void EmitThrow<TException>(this ILGenerator ilGenerator)
        where TException : Exception
        => ilGenerator.Emit(OpCodes.Throw, typeof(TException).GetConstructor(Type.EmptyTypes));
}
