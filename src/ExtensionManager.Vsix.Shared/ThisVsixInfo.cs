namespace ExtensionManager;

public sealed class ThisVsixInfo : IThisVsixInfo
{
    public string Id => Vsix.Id;
    public string Name => Vsix.Name;
    public string Description => Vsix.Description;
    public string Language => Vsix.Language;
    public string Version => Vsix.Version;
    public string Author => Vsix.Author;
    public string Tags => Vsix.Tags;
}
