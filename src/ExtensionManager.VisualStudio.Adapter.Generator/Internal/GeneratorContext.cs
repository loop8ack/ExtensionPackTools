using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using ExtensionManager.VisualStudio.Adapter.Generator.Internal.Emitter;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal;

internal sealed class GeneratorContext
{
    public GeneratorReflector Reflect { get; }
    public ModuleEmitter Emit { get; }
    public string RootNamespace { get; }

    public GeneratorContext(IReadOnlyList<Assembly> assemblies, ModuleBuilder moduleBuilder, string rootNamespace)
    {
        Reflect = new(assemblies);
        Emit = new(moduleBuilder);
        RootNamespace = rootNamespace;
    }

    public Type EmitType(ITypeGenerator generator)
        => generator.Emit(this);
}
