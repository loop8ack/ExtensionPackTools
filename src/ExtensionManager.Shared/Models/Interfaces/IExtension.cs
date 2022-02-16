namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a POCO that encapsulates
    /// metadata about an extension.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets or sets a string containing the URL from which the extension can be
        /// downloaded.
        /// </summary>
        string DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the extension.
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Gets or sets the URL of the page on the Visual Studio Marketplace website for
        /// the extension.
        /// </summary>
        string MoreInfoUrl { get; set; }

        /// <summary>
        /// Gets or sets a string containing the name of the extension.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this extension is selected.
        /// </summary>
        bool Selected { get; set; }
    }
}