namespace ExtensionManager;

public interface IThisVsixInfo
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    string Language { get; }
    string Version { get; }
    string Author { get; }
    string Tags { get; }
}
