using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using ExtensionManager.Manifest.Internal.Models;
using ExtensionManager.VisualStudio.Extensions;

namespace ExtensionManager.Manifest.Internal.Versions;

internal class V0ManifestVersion : ManifestVersion
{
    public V0ManifestVersion()
        : base(new(0, 0))
    {
    }

    public override async Task<IManifest> ReadAsync(Stream stream)
    {
        var data = await JsonSerializer
            .DeserializeAsync<JsonManifest>(stream, (JsonSerializerOptions?)null)
            .ConfigureAwait(false);

        if (data is null)
            throw ThrowHelper.CreateCannotReadManifestException();

        return CreateManifestDto(data);

        static ManifestDto CreateManifestDto(JsonManifest data)
        {
            var extensions = new List<IVSExtension>();

            if (data.Extensions?.Optional is not null)
            {
                foreach (var ext in data.Extensions.Optional)
                    extensions.Add(CreateExtensionDto(ext));
            }

            if (data.Extensions?.Mandatory is not null)
            {
                foreach (var ext in data.Extensions.Mandatory)
                    extensions.Add(CreateExtensionDto(ext));
            }

            return new ManifestDto()
            {
                Id = Guid.NewGuid(),
                Name = "Legacy file",
                Description = "Legacy file",
                Extensions = extensions,
            };
        }

        static ExtensionDto CreateExtensionDto(JsonExtension data)
        {
            return new ExtensionDto()
            {
                Id = data.ID ?? throw ThrowHelper.CreateExtensionDataHasNoVsixIdException(),
                Name = data.Name,
                MoreInfoURL = data.MoreInfoURL,
                DownloadUrl = null,
            };
        }
    }

    public override Task WriteAsync(Stream stream, IManifest manifest, CancellationToken cancellationToken)
        => throw CreateVersionNotSupportedException();
}

file sealed class JsonManifest
{
    [JsonPropertyName("extensions")]
    public JsonManifestExtensions? Extensions { get; set; }
}

file sealed class JsonManifestExtensions
{
    [JsonPropertyName("mandatory")]
    public IEnumerable<JsonExtension>? Mandatory { get; set; }

    [JsonPropertyName("optional")]
    public IEnumerable<JsonExtension>? Optional { get; set; }
}

file sealed class JsonExtension
{
    [JsonPropertyName("productId")]
    public string? ID { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("link")]
    public string? MoreInfoURL { get; set; }
}
