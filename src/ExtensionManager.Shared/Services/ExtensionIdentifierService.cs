namespace ExtensionManager
{
    /// <summary>
    /// Exposes methods to obtain information from external data sources (UI/UX, Visual
    /// Studio, etc.)
    /// </summary>
    /// <remarks>
    /// This class is in charge of getting the identifiers of those extensions
    /// that are (a) obtained from the Visual Studio Marketplace, (b) are not system
    /// components, and (c) is not a component of a package (<c>IsPackComponent</c> is
    /// <see langword="false" />).
    /// </remarks>
    public class ExtensionIdentifierService { }
}