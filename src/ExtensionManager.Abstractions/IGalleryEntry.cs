namespace ExtensionManager
{
    public interface IGalleryEntry
    {
        string VsixID { get; }
        string Name { get; }
        string MoreInfoURL { get; }
        string DownloadUrl { get; }
    }
}
