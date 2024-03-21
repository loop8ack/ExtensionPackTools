using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;

internal sealed class PropertyEmitter
{
    private readonly ClassEmitter _typeEmitter;
    private readonly PropertyBuilder _builder;

    public PropertyEmitter(ClassEmitter typeEmitter, PropertyBuilder builder)
    {
        _typeEmitter = typeEmitter;
        _builder = builder;
    }

    public MethodBuilder GetField(FieldBuilder fieldBuilder, MethodAttributes attributes)
    {
        return Get(attributes)
            .EmitIL(il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            });
    }

    public MethodBuilder SetField(FieldBuilder fieldBuilder, MethodAttributes attributes)
    {
        return Set(attributes)
            .EmitIL(il =>
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, fieldBuilder);
                il.Emit(OpCodes.Ret);
            });
    }

    public MethodBuilder Get(MethodAttributes attributes)
    {
        var builder = _typeEmitter.Method(
            $"get_{_builder.Name}",
            attributes,
            _builder.PropertyType,
            []);

        _builder.SetGetMethod(builder);

        return builder;
    }

    public MethodBuilder Set(MethodAttributes attributes)
    {
        var builder = _typeEmitter.Method(
            $"set_{_builder.Name}",
            attributes,
            typeof(void),
            [_builder.PropertyType]);

        _builder.SetGetMethod(builder);

        return builder;
    }
}
