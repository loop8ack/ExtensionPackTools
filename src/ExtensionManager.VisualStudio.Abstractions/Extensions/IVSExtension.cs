namespace ExtensionManager.VisualStudio.Extensions;

public interface IVSExtension
{
    string Id { get; }
    string? Name { get; }
    string? MoreInfoURL { get; }
    string? DownloadUrl { get; }
}
