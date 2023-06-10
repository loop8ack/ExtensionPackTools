using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest.Internal.Models;

internal sealed class ExtensionDto : IVSExtension
{
    public required string Id { get; init; }
    public required string? Name { get; init; }
    public required string? MoreInfoURL { get; init; }
    public required string? DownloadUrl { get; init; }
}
