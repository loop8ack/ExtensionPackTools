using System;
using System.Linq;
using System.Reflection;

using ExtensionManager.VisualStudio.Adapter.Generator.Internal;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;
using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Types.Extensions;

internal sealed class GalleryExtensionGenerator : ITypeGenerator
{
    private readonly string _baseTypeName;

    public GalleryExtensionGenerator(string baseTypeName)
        => _baseTypeName = baseTypeName;

    public Type Emit(GeneratorContext context)
    {
        const BindingFlags searchBindingFlags = 0
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.FlattenHierarchy;

        var baseType = context.Reflect.GetType($"Microsoft.VisualStudio.ExtensionManager.{_baseTypeName}");

        return context.Emit.Class($"{context.RootNamespace}.Extensions.<>VSGalleryExtension", baseType, emit =>
            {
                emit.Implement(typeof(IVSExtension));

                var properties = baseType.GetProperties(searchBindingFlags);

                foreach (var property in properties)
                    emit.OverrideProperty(property);

                foreach (var method in baseType.GetMethods(searchBindingFlags))
                {
                    if (properties.Any(x => x.GetMethod == method || x.SetMethod == method))
                        continue;

                    emit.OverrideMethod(method);
                }

                emit.DefaultCtor();
            });
    }
}

file static class Extensions
{
    public static void OverrideProperty(this ClassEmitter classEmitter, PropertyInfo property)
    {
        var isAbstract = (property.GetMethod ?? property.SetMethod)?.IsAbstract ?? false;

        if (!isAbstract)
            return;

        if (property.GetMethod is not null && property.SetMethod is not null)
        {
            var fieldBuilder = classEmitter.Field($"<{property.Name}>k__BackingField", property.PropertyType, FieldAttributes.Private | FieldAttributes.InitOnly);

            classEmitter.Property(property.Name, property.PropertyType,
                emit =>
                {
                    emit.GetField(fieldBuilder, property.GetMethod.GetMethodAttributes());
                    emit.SetField(fieldBuilder, property.SetMethod.GetMethodAttributes());
                });
        }
        else
        {
            classEmitter.Property(property.Name, property.PropertyType,
                emit =>
                {
                    if (property.GetMethod is not null)
                        emit.Get(property.GetMethod.GetMethodAttributes()).Throws<NotImplementedException>();

                    if (property.SetMethod is not null)
                        emit.Set(property.SetMethod.GetMethodAttributes()).Throws<NotImplementedException>();
                });
        }
    }

    public static void OverrideMethod(this ClassEmitter classEmitter, MethodInfo method)
    {
        if (!method.IsAbstract)
            return;

        var methodAttributes = method.GetMethodAttributes();
        var parameterTypes = method.GetParameters().SelectArray(p => p.ParameterType);

        classEmitter
            .Method(method.Name, methodAttributes, method.ReturnType, parameterTypes)
            .Throws<NotImplementedException>();
    }
}
