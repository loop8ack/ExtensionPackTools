namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal.Utils;

internal static class ReflectionEmitExtensions
{
    public static MethodBuilder EmitIL(this MethodBuilder builder, Action<ILGenerator> generateIL)
    {
        generateIL(builder.GetILGenerator());
        return builder;
    }

    public static void Throws<TException>(this MethodBuilder builder)
        where TException : Exception
    {
        builder.EmitIL(il => il
            .Emit(OpCodes.Throw, typeof(TException).GetConstructor(Type.EmptyTypes)));
    }

    public static ConstructorBuilder EmitIL(this ConstructorBuilder builder, Action<ILGenerator> generateIL)
    {
        generateIL(builder.GetILGenerator());
        return builder;
    }

    public static void EmitCallEmptyBaseCtor(this ILGenerator il, Type baseType)
    {
        const BindingFlags searchCtorAttributes = 0
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance;

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Call, baseType.GetConstructor(searchCtorAttributes, null, [], []));
    }
}
