using System;

namespace ExtensionManager
{
    public interface IGalleryEntry
    {
        string Author { get; set; }
        string DefaultName { get; set; }
        string Description { get; set; }
        int DownloadCount { get; set; }
        string DownloadUpdateUrl { get; set; }
        string DownloadUrl { get; set; }
        string Icon { get; set; }
        string Id { get; set; }
        bool IsSelected { get; set; }
        DateTime LastModified { get; set; }
        string MoreInfoURL { get; set; }
        string Name { get; set; }
        string OnlinePreviewImage { get; set; }
        float Priority { get; }
        string ProjectTypeFriendly { get; set; }
        double Rating { get; set; }
        int RatingsCount { get; set; }
        string ReferralUrl { get; set; }
        string ReportAbuseUrl { get; set; }
        bool SupportsCodeSeparation { get; set; }
        bool SupportsMasterPage { get; set; }
        string Type { get; set; }
        string VsixID { get; set; }
        string VsixReferences { get; set; }
        string VsixVersion { get; set; }
        string ToString();
    }
}