using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;

internal sealed class ClassEmitter
{
    private readonly TypeBuilder _builder;

    public ClassEmitter(TypeBuilder builder)
        => _builder = builder;

    public void Implement(Type type)
        => _builder.AddInterfaceImplementation(type);

    public void Implement<TInterface>(Action<InterfaceImplementationEmitter<TInterface>> emit)
    {
        _builder.AddInterfaceImplementation(typeof(TInterface));
        emit(new InterfaceImplementationEmitter<TInterface>(this));
    }

    public FieldBuilder Field(string name, Type type, FieldAttributes attributes)
        => _builder.DefineField(name, type, attributes);

    public PropertyBuilder Property(string name, Type type, Action<PropertyEmitter> emit)
    {
        var builder = _builder.DefineProperty(name, PropertyAttributes.None, type, null);
        emit(new PropertyEmitter(this, builder));
        return builder;
    }

    public MethodBuilder ImplementMethod(MethodInfo methodInfo)
    {
        var methodAttributes = methodInfo.GetMethodAttributes();
        var parameterTypes = methodInfo.GetParameters().SelectArray(p => p.ParameterType);

        return Method(methodInfo.Name, methodAttributes, methodInfo.ReturnType, parameterTypes);
    }

    public MethodBuilder Method(string name, MethodAttributes attributes, Type returnType, Type[] parameters)
        => _builder.DefineMethod(name, attributes, returnType, parameters);

    public void AutoCtor(FieldBuilder field)
    {
        Ctor(field.FieldType)
            .EmitIL(il =>
            {
                il.EmitCallEmptyBaseCtor(_builder.BaseType);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Stfld, field);
                il.Emit(OpCodes.Ret);
            });
    }

    public ConstructorBuilder Ctor(params Type[] parameters)
    {
        const MethodAttributes ctorAttributes = MethodAttributes.Public
            | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName
            | MethodAttributes.RTSpecialName;

        return _builder.DefineConstructor(ctorAttributes, CallingConventions.Standard, parameters);
    }

    public ConstructorBuilder DefaultCtor()
    {
        return Ctor(parameters: [])
            .EmitIL(il =>
            {
                il.EmitCallEmptyBaseCtor(_builder.BaseType);
                il.Emit(OpCodes.Ret);
            });
    }
}
