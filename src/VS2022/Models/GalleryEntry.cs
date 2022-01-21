using System;
using Microsoft.VisualStudio.ExtensionManager;

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

    public class GalleryEntry : GalleryOnlineExtension, IRepositoryEntry, IGalleryEntry
    {
        public override string Author { get; set; }
        public override string DefaultName { get; set; }
        public override string Description { get; set; }
        public override int DownloadCount { get; set; }
        public override string DownloadUpdateUrl { get; set; }
        public override string DownloadUrl { get; set; }
        public override string Icon { get; set; }
        public override string Id { get; set; }
        public override bool IsSelected { get; set; }
        public override DateTime LastModified { get; set; }
        public override string MoreInfoURL { get; set; }
        public override string Name { get; set; }
        public override string OnlinePreviewImage { get; set; }
        public override float Priority => throw new NotImplementedException();
        public override string ProjectTypeFriendly { get; set; }
        public override double Rating { get; set; }
        public override int RatingsCount { get; set; }
        public override string ReferralUrl { get; set; }
        public override string ReportAbuseUrl { get; set; }
        public override bool SupportsCodeSeparation { get; set; }
        public override bool SupportsMasterPage { get; set; }
        public override string Type { get; set; }
        public override string VsixID { get; set; }
        public override string VsixReferences { get; set; }
        public override string VsixVersion { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}