using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest.Internal.Versions;

namespace ExtensionManager.Manifest.Internal;

internal abstract class ManifestVersion
{
    public const string VersionPropertyName = "version";

    private static readonly SortedList<Version, ManifestVersion> _versions;

    public static ManifestVersion First { get; }
    public static ManifestVersion Latest { get; }

    static ManifestVersion()
    {
        _versions = new SortedList<Version, ManifestVersion>();

        AddVersion<V0ManifestVersion>(_versions);
        AddVersion<V1ManifestVersion>(_versions);

        First = _versions.First().Value;
        Latest = _versions.Last().Value;

        static void AddVersion<TVersion>(SortedList<Version, ManifestVersion> versionsList)
            where TVersion : ManifestVersion, new()
        {
            TVersion version = new();

            versionsList.Add(version.Version, version);
        }
    }

    public static async Task<ManifestVersion> FindAsync(Stream stream)
    {
        var document = await JsonDocument.ParseAsync(stream);

        if (!document.RootElement.TryGetProperty(VersionPropertyName, out var versionProperty))
            return First;

        var value = versionProperty.GetString();

        if (!Version.TryParse(value, out var version))
            return First;

        ManifestVersion? foundVersion = null;

        foreach (var item in _versions.Values)
        {
            if (version >= item.Version)
                foundVersion = item;
            else
                break;
        }

        return foundVersion
            ?? throw new InvalidOperationException($"Unknown manifest file version: {version}");
    }

    public Version Version { get; }

    protected ManifestVersion(Version version)
    {
        Version = version;
    }

    public abstract Task<IManifest> ReadAsync(Stream stream);
    public abstract Task WriteAsync(Stream stream, IManifest manifest, CancellationToken cancellationToken);

    protected Exception CreateVersionNotSupportedException()
        => ThrowHelper.CreateManifestVersionNotSupportedException(Version);
}
