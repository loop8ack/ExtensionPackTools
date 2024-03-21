using System;

namespace ExtensionManager.VisualStudio.Adapter.Generator.Internal;

internal interface ITypeGenerator
{
    Type Emit(GeneratorContext context);
}
