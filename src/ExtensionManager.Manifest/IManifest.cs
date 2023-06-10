using System;
using System.Collections.Generic;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest;

public interface IManifest
{
    Guid Id { get; }
    string? Name { get; set; }
    string? Description { get; set; }
    IList<IVSExtension> Extensions { get; }
}
