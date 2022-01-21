using System;
using ExtensionManager.Core.Models.Interfaces;
using Microsoft.VisualStudio.ExtensionManager;

namespace ExtensionManager
{
    /// <summary>
    /// Encapsulates the metadata for extensions that have been installed from the
    /// Visual Studio Marketplace.
    /// </summary>
    public class GalleryEntry : GalleryOnlineExtension, IRepositoryEntry, IGalleryEntry
    {
        /// <summary>
        /// Gets or sets the name of the author of the extension.
        /// </summary>
        public override string Author { get; set; }

        /// <summary>
        /// Gets or sets the default name of the extension.
        /// </summary>
        public override string DefaultName { get; set; }

        /// <summary>
        /// Gets or sets the extension's description.
        /// </summary>
        public override string Description { get; set; }

        /// <summary>
        /// Gets or sets a count of the number of times that this extension has been
        /// downloaded from the Visual Studio Marketplace.
        /// </summary>
        public override int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL from which this extension's
        /// <c>.vsix</c> file can be directly downloaded.
        /// </summary>
        public override string DownloadUpdateUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL from which this extension's
        /// <c>.vsix</c> file can be directly downloaded.
        /// </summary>
        public override string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL of the icon for this extension.
        /// </summary>
        public override string Icon { get; set; }

        /// <summary>
        /// Gets or sets a string containing the identifier of this extension.
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension is selected.
        /// </summary>
        public override bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="T:System.DateTime" /> value that indicates when this
        /// extension was last modified.
        /// </summary>
        public override DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the URL on the Visual Studio Marketplace where users can navigate
        /// in order to obtain more information about this extension.
        /// </summary>
        public override string MoreInfoURL { get; set; }

        /// <summary>
        /// Gets or sets this extension's name.
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL to the extension's online preview
        /// image.
        /// </summary>
        public override string OnlinePreviewImage { get; set; }

        /// <summary>
        /// Gets or sets the priority of the extension.
        /// </summary>
        public override float Priority
            => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets a string containing project information for the extension.
        /// </summary>
        public override string ProjectTypeFriendly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the average 1-5 star rating of this extension
        /// by other users.
        /// </summary>
        public override double Rating { get; set; }

        /// <summary>
        /// Gets or sets a count of the ratings that have been provided for this extension.
        /// </summary>
        public override int RatingsCount { get; set; }

        /// <summary>
        /// Gets or sets a URL to be used for HTTP referrals.
        /// </summary>
        public override string ReferralUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to use for reporting abuse by this extension.
        /// </summary>
        public override string ReportAbuseUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension supports Code
        /// Separation.
        /// </summary>
        public override bool SupportsCodeSeparation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension supports a master page.
        /// </summary>
        public override bool SupportsMasterPage { get; set; }

        /// <summary>
        /// Gets or sets a string containing information about this extension's type.
        /// </summary>
        public override string Type { get; set; }

        /// <summary>
        /// Gets or sets a string containing information about the VSIX ID of this
        /// extension.
        /// </summary>
        public override string VsixID { get; set; }

        /// <summary>
        /// Gets or sets a string containing information on the references to this
        /// extension.
        /// </summary>
        public override string VsixReferences { get; set; }

        /// <summary>
        /// Gets or sets a string containing the version number of this extension.
        /// </summary>
        public override string VsixVersion { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}