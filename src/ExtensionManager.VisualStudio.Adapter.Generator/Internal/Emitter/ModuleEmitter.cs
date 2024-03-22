namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;

internal sealed class ModuleEmitter
{
    private readonly ModuleBuilder _builder;

    public ModuleEmitter(ModuleBuilder builder)
        => _builder = builder;

    public Type Class(string fullName, Action<ClassEmitter> emit)
        => Class(fullName, typeof(object), emit);

    public Type Class(string fullName, Type baseType, Action<ClassEmitter> emit)
    {
        const TypeAttributes typeAttributes = 0
            | TypeAttributes.Public
            | TypeAttributes.Sealed
            | TypeAttributes.Class
            | TypeAttributes.BeforeFieldInit;

        baseType ??= typeof(object);

        var typeBuilder = _builder.DefineType(fullName, typeAttributes, baseType);

        emit(new ClassEmitter(typeBuilder));

        return typeBuilder.CreateType();
    }
}
