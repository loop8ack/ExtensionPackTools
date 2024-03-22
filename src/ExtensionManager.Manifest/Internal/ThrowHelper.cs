namespace ExtensionManager.Manifest.Internal;

internal static class ThrowHelper
{
    public static Exception CreateCannotReadManifestException()
        => new InvalidOperationException($"Cannot read manifest file");

    public static Exception CreateExtensionDataHasNoVsixIdException()
        => new InvalidOperationException($"Extension data in json file has no vsix id");

    public static Exception CreateManifestVersionNotSupportedException(Version version)
        => new NotSupportedException($"Writing file version {version} is no longer supported.");
}
