using ExtensionManager.Manifest.Internal.Models;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest.Internal;

internal sealed class ManifestService : IManifestService
{
    public IManifest CreateNew()
    {
        return new ManifestDto()
        {
            Id = Guid.NewGuid(),
            Name = "My Visual Studio extensions",
            Description = "A collection of my Visual Studio extensions",
            Extensions = new List<IVSExtension>()
        };
    }

    public async Task<IManifest> ReadAsync(string filePath)
    {
        using var stream = File.OpenRead(filePath);

        try
        {
            return await ManifestVersion.Latest.ReadAsync(stream).ConfigureAwait(false);
        }
        catch
        {
            stream.Position = 0;

            var foundVersion = await ManifestVersion.FindAsync(stream).ConfigureAwait(false);

            if (foundVersion == ManifestVersion.Latest)
                throw;

            stream.Position = 0;

            return await foundVersion.ReadAsync(stream).ConfigureAwait(false);
        }
    }

    public async Task WriteAsync(string filePath, IManifest manifest, CancellationToken cancellationToken)
    {
        var directoryPath = Path.GetDirectoryName(filePath);

        if (string.IsNullOrWhiteSpace(directoryPath))
            directoryPath = ".";

        Directory.CreateDirectory(directoryPath);

        using var stream = File.Create(filePath);

        await ManifestVersion.Latest.WriteAsync(stream, manifest, cancellationToken);
    }
}
