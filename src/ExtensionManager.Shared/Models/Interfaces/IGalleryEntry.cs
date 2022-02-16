using System;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of an object that exposes
    /// the metadata for an installed extension that was obtained from the Visual
    /// Studio Marketplace..
    /// </summary>
    /// <remarks>
    /// This interface exposes the smallest common set of properties across
    /// all the versions of Visual Studio that this extension supports.
    /// </remarks>
    public interface IGalleryEntry
    {
        /// <summary>
        /// Gets or sets the name of the author of the extension.
        /// </summary>
        string Author { get; set; }

        /// <summary>
        /// Gets or sets the default name of the extension.
        /// </summary>
        string DefaultName { get; set; }

        /// <summary>
        /// Gets or sets the extension's description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets a count of the number of times that this extension has been
        /// downloaded from the Visual Studio Marketplace.
        /// </summary>
        int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL from which this extension's
        /// <c>.vsix</c> file can be directly downloaded.
        /// </summary>
        string DownloadUpdateUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL from which this extension's
        /// <c>.vsix</c> file can be directly downloaded.
        /// </summary>
        string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL of the icon for this extension.
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// Gets or sets a string containing the identifier of this extension.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension is selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="T:System.DateTime" /> value that indicates when this
        /// extension was last modified.
        /// </summary>
        DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets the URL on the Visual Studio Marketplace where users can navigate
        /// in order to obtain more information about this extension.
        /// </summary>
        string MoreInfoURL { get; set; }

        /// <summary>
        /// Gets or sets this extension's name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a string containing the URL to the extension's online preview
        /// image.
        /// </summary>
        string OnlinePreviewImage { get; set; }

        /// <summary>
        /// Gets or sets the priority of the extension.
        /// </summary>
        float Priority { get; }

        /// <summary>
        /// Gets or sets a string containing project information for the extension.
        /// </summary>
        string ProjectTypeFriendly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the average 1-5 star rating of this extension
        /// by other users.
        /// </summary>
        double Rating { get; set; }

        /// <summary>
        /// Gets or sets a count of the ratings that have been provided for this extension.
        /// </summary>
        int RatingsCount { get; set; }

        /// <summary>
        /// Gets or sets a URL to be used for HTTP referrals.
        /// </summary>
        string ReferralUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to use for reporting abuse by this extension.
        /// </summary>
        string ReportAbuseUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension supports Code
        /// Separation.
        /// </summary>
        bool SupportsCodeSeparation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension supports a master page.
        /// </summary>
        bool SupportsMasterPage { get; set; }

        /// <summary>
        /// Gets or sets a string containing information about this extension's type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets a string containing information about the VSIX ID of this
        /// extension.
        /// </summary>
        string VsixID { get; set; }

        /// <summary>
        /// Gets or sets a string containing information on the references to this
        /// extension.
        /// </summary>
        string VsixReferences { get; set; }

        /// <summary>
        /// Gets or sets a string containing the version number of this extension.
        /// </summary>
        string VsixVersion { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        string ToString();
    }
}