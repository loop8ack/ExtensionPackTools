using System;
using System.Collections.Generic;

using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest.Internal.Models;

internal sealed class ManifestDto : IManifest
{
    public required Guid Id { get; init; }
    public required string? Name { get; set; }
    public required string? Description { get; set; }
    public required IList<IVSExtension> Extensions { get; init; }
}
